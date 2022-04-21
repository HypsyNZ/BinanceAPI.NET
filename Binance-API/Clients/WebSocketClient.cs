using BinanceAPI.Interfaces;
using BinanceAPI.Objects;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static BinanceAPI.Logging;

namespace BinanceAPI
{
    /// <summary>
    /// A wrapper around the ClientWebSocket
    /// </summary>
    public class WebSocketClient : IWebsocket
    {
        internal static int lastStreamId;
        private static readonly object streamIdLock = new();

        private ClientWebSocket _socket;
        private Task? _sendTask;
        private Task? _receiveTask;
        private Task? _timeoutTask;
        private readonly AsyncResetEvent _sendEvent;
        private readonly ConcurrentQueue<byte[]> _sendBuffer;
        private readonly IDictionary<string, string> cookies;
        private readonly IDictionary<string, string> headers;
        private CancellationTokenSource _ctsSource;
        private bool _closing;
        private bool _startedSent;
        private bool _startedReceive;

        private readonly List<DateTime> _outgoingMessages;
        private DateTime _lastReceivedMessagesUpdate;

        /// <summary>
        /// Received messages time -> size
        /// </summary>
        protected readonly List<ReceiveItem> _receivedMessages;

        /// <summary>
        /// Received messages lock
        /// </summary>
        protected readonly object _receivedMessagesLock;

        /// <summary>
        /// Handlers for when an error happens on the socket
        /// </summary>
        protected readonly List<Action<Exception>> errorHandlers = new();

        /// <summary>
        /// Handlers for when the socket connection is opened
        /// </summary>
        protected readonly List<Action> openHandlers = new();

        /// <summary>
        /// Handlers for when the connection is closed
        /// </summary>
        protected readonly List<Action> closeHandlers = new();

        /// <summary>
        /// Handlers for when a message is received
        /// </summary>
        protected readonly List<Action<string>> messageHandlers = new();

        /// <summary>
        /// The id of this socket
        /// </summary>
        public int Id { get; }

        /// <inheritdoc />
        public string? Origin { get; set; }

        /// <summary>
        /// Whether this socket is currently reconnecting
        /// </summary>
        public bool Reconnecting { get; set; }

        /// <summary>
        /// The timestamp this socket has been active for the last time
        /// </summary>
        public DateTime LastActionTime { get; private set; }

        /// <summary>
        /// Delegate used for processing byte data received from socket connections before it is processed by handlers
        /// </summary>
        public Func<byte[], string>? DataInterpreterBytes { get; set; }

        /// <summary>
        /// Delegate used for processing string data received from socket connections before it is processed by handlers
        /// </summary>
        public Func<string, string>? DataInterpreterString { get; set; }

        /// <summary>
        /// Url this socket connects to
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// If the connection is closed
        /// </summary>
        public bool IsClosed => _socket.State == WebSocketState.Closed;

        /// <summary>
        /// If the connection is open
        /// </summary>
        public bool IsOpen => _socket.State == WebSocketState.Open && !_closing;

        /// <summary>
        /// Ssl protocols supported. NOT USED BY THIS IMPLEMENTATION
        /// </summary>
        public SslProtocols SSLProtocols { get; set; }

        private Encoding _encoding = Encoding.UTF8;

        /// <summary>
        /// Encoding used for decoding the received bytes into a string
        /// </summary>
        public Encoding? Encoding
        {
            get => _encoding;
            set
            {
                if (value != null)
                    _encoding = value;
            }
        }

        /// <summary>
        /// The timespan no data is received on the socket. If no data is received within this time an error is generated
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// The current kilobytes per second of data being received, averaged over the last 3 seconds
        /// </summary>
        public double IncomingKbps
        {
            get
            {
                lock (_receivedMessagesLock)
                {
                    UpdateReceivedMessages();

                    if (!_receivedMessages.Any())
                        return 0;

                    return Math.Round(_receivedMessages.Sum(v => v.Bytes) / 1000 / 3d);
                }
            }
        }

        /// <summary>
        /// Socket closed event
        /// </summary>
        public event Action OnClose
        {
            add => closeHandlers.Add(value);
            remove => closeHandlers.Remove(value);
        }

        /// <summary>
        /// Socket message received event
        /// </summary>
        public event Action<string> OnMessage
        {
            add => messageHandlers.Add(value);
            remove => messageHandlers.Remove(value);
        }

        /// <summary>
        /// Socket error event
        /// </summary>
        public event Action<Exception> OnError
        {
            add => errorHandlers.Add(value);
            remove => errorHandlers.Remove(value);
        }

        /// <summary>
        /// Socket opened event
        /// </summary>
        public event Action OnOpen
        {
            add => openHandlers.Add(value);
            remove => openHandlers.Remove(value);
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="url">The url the socket should connect to</param>
        public WebSocketClient(string url) : this(url, new Dictionary<string, string>(), new Dictionary<string, string>())
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="url">The url the socket should connect to</param>
        /// <param name="cookies">Cookies to sent in the socket connection request</param>
        /// <param name="headers">Headers to sent in the socket connection request</param>
        public WebSocketClient(string url, IDictionary<string, string> cookies, IDictionary<string, string> headers)
        {
            Id = NextStreamId();
            Url = url;
            this.cookies = cookies;
            this.headers = headers;

            _outgoingMessages = new List<DateTime>();
            _receivedMessages = new List<ReceiveItem>();
            _sendEvent = new AsyncResetEvent();
            _sendBuffer = new ConcurrentQueue<byte[]>();
            _ctsSource = new CancellationTokenSource();
            _receivedMessagesLock = new object();

            _socket = CreateSocket();
        }

        /// <summary>
        /// Set a proxy to use. Should be set before connecting
        /// </summary>
        /// <param name="proxy"></param>
        public virtual void SetProxy(ApiProxy proxy)
        {
            Uri.TryCreate($"{proxy.Host}:{proxy.Port}", UriKind.Absolute, out var uri);
            _socket.Options.Proxy = uri?.Scheme == null
                ? _socket.Options.Proxy = new WebProxy(proxy.Host, proxy.Port)
                : _socket.Options.Proxy = new WebProxy
                {
                    Address = uri
                };

            if (proxy.Login != null)
                _socket.Options.Proxy.Credentials = new NetworkCredential(proxy.Login, proxy.Password);
        }

        /// <summary>
        /// Connect the websocket
        /// </summary>
        /// <returns>True if successfull</returns>
        public virtual async Task<bool> ConnectAsync()
        {
#if DEBUG
            SocketLog?.Debug($"Socket {Id} connecting");
#endif
            try
            {
                using CancellationTokenSource tcs = new(TimeSpan.FromSeconds(10));
                await _socket.ConnectAsync(new Uri(Url), default).ConfigureAwait(false);

                Handle(openHandlers);
            }
            catch
#if DEBUG
            (Exception e)
#endif
            {
#if DEBUG
                SocketLog?.Debug($"Socket {Id} connection failed: " + e.ToLogString());
#endif
                return false;
            }
#if DEBUG
            SocketLog?.Trace($"Socket {Id} connection succeeded, starting communication");
#endif
            _sendTask = Task.Factory.StartNew(SendLoopAsync, default, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
            _receiveTask = Task.Factory.StartNew(ReceiveLoopAsync, default, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();

            if (Timeout != default) { _timeoutTask = Task.Run(CheckTimeoutAsync); }
            var sw = Stopwatch.StartNew();
            while (!_startedSent || !_startedReceive)
            {
                // Wait for the tasks to have actually started
                await Task.Delay(7).ConfigureAwait(false);

                if (sw.ElapsedMilliseconds > 5000)
                {
                    _ = _socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", default);
#if DEBUG
                    SocketLog?.Debug($"Socket {Id} startup interupted");
#endif
                    return false;
                }
            }
#if DEBUG
            SocketLog?.Debug($"Socket {Id} connected");
#endif
            return true;
        }

        /// <summary>
        /// Send data over the websocket
        /// </summary>
        /// <param name="data">Data to send</param>
        public virtual void Send(string data)
        {
            if (_closing)
                throw new InvalidOperationException($"Socket {Id} Can't send data when socket is not connected");

            var bytes = _encoding.GetBytes(data);
#if DEBUG
            SocketLog?.Trace($"Socket {Id} Adding {bytes.Length} to sent buffer");
#endif
            _sendBuffer.Enqueue(bytes);
            _sendEvent.Set();
        }

        /// <summary>
        /// Close the websocket
        /// </summary>
        /// <returns></returns>
        public virtual async Task CloseAsync()
        {
#if DEBUG
            SocketLog?.Debug($"Socket {Id} closing");
#endif
            await CloseInternalAsync(true, true).ConfigureAwait(false);
        }

        /// <summary>
        /// Internal close method, will wait for each task to complete to gracefully close
        /// </summary>
        /// <param name="waitSend"></param>
        /// <param name="waitReceive"></param>
        /// <returns></returns>
        private async Task CloseInternalAsync(bool waitSend, bool waitReceive)
        {
            if (_closing)
                return;

            _closing = true;
            var tasksToAwait = new List<Task>();
            if (_socket.State == WebSocketState.Open)
                tasksToAwait.Add(_socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing", default));

            _ctsSource.Cancel();
            _sendEvent.Set();
            if (waitSend)
                tasksToAwait.Add(_sendTask!);
            if (waitReceive)
                tasksToAwait.Add(_receiveTask!);
            if (_timeoutTask != null)
                tasksToAwait.Add(_timeoutTask);
#if DEBUG
            SocketLog?.Trace($"Socket {Id} waiting for communication loops to finish");
#endif
            await Task.WhenAll(tasksToAwait).ConfigureAwait(false);
#if DEBUG
            SocketLog?.Debug($"Socket {Id} closed");
#endif
            Handle(closeHandlers);
        }

        /// <summary>
        /// Dispose the socket
        /// </summary>
        public void Dispose()
        {
#if DEBUG
            SocketLog?.Debug($"Socket {Id} disposing");
#endif
            _socket.Dispose();
            _ctsSource.Dispose();

            errorHandlers.Clear();
            openHandlers.Clear();
            closeHandlers.Clear();
            messageHandlers.Clear();
#if DEBUG
            SocketLog?.Trace($"Socket {Id} disposed");
#endif
        }

        /// <summary>
        /// Reset the socket so a new connection can be attempted after it has been connected before
        /// </summary>
        public void Reset()
        {
#if DEBUG
            SocketLog?.Debug($"Socket {Id} resetting");
#endif
            _ctsSource = new CancellationTokenSource();
            _closing = false;

            while (_sendBuffer.TryDequeue(out _)) { } // Clear send buffer

            _socket = CreateSocket();
        }

        /// <summary>
        /// Create the socket object
        /// </summary>
        private ClientWebSocket CreateSocket()
        {
            var cookieContainer = new CookieContainer();
            foreach (var cookie in cookies)
                cookieContainer.Add(new Cookie(cookie.Key, cookie.Value));

            var socket = new ClientWebSocket();
            socket.Options.Cookies = cookieContainer;
            foreach (var header in headers)
                socket.Options.SetRequestHeader(header.Key, header.Value);
            socket.Options.KeepAliveInterval = TimeSpan.FromSeconds(10);
            socket.Options.SetBuffer(65536, 65536); // Setting it to anything bigger than 65536 throws an exception in .net framework
            return socket;
        }

        /// <summary>
        /// Loop for sending data
        /// </summary>
        /// <returns></returns>
        private async Task SendLoopAsync()
        {
            _startedSent = true;
            try
            {
                while (!_ctsSource.Token.IsCancellationRequested)
                {
                    if (_closing)
                        break;

                    await _sendEvent.WaitAsync().ConfigureAwait(false);

                    if (_closing)
                        break;

                    while (_sendBuffer.TryDequeue(out var data))
                    {
                        try
                        {
                            await _socket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), WebSocketMessageType.Text, true, _ctsSource.Token).ConfigureAwait(false);
                            _outgoingMessages.Add(DateTime.UtcNow);
#if DEBUG
                            SocketLog?.Trace($"Socket {Id} sent {data.Length} bytes");
#endif
                        }
                        catch (Exception)
                        {
                            _ = Task.Run(async () => await CloseInternalAsync(false, true).ConfigureAwait(false));
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                _startedSent = false;
            }
        }

        /// <summary>
        /// Loop for receiving and reassembling data
        /// </summary>
        /// <returns></returns>
        private async Task ReceiveLoopAsync()
        {
            _startedReceive = true;

            var buffer = new ArraySegment<byte>(new byte[65536]);
            var received = 0;
            try
            {
                while (!_ctsSource.Token.IsCancellationRequested)
                {
                    if (_closing)
                        break;

                    MemoryStream? memoryStream = null;
                    WebSocketReceiveResult? receiveResult = null;
                    bool multiPartMessage = false;
                    while (!_ctsSource.Token.IsCancellationRequested)
                    {
                        try
                        {
                            receiveResult = await _socket.ReceiveAsync(buffer, _ctsSource.Token).ConfigureAwait(false);
                            received += receiveResult.Count;
                            lock (_receivedMessagesLock)
                                _receivedMessages.Add(new ReceiveItem(DateTime.UtcNow, receiveResult.Count));

                            if (receiveResult.MessageType == WebSocketMessageType.Close)
                            {
                                // Connection closed unexpectedly
#if DEBUG
                                SocketLog?.Debug($"Socket {Id} received `Close` message");
#endif
                                await CloseInternalAsync(true, false).ConfigureAwait(false);
                                break;
                            }

                            if (!receiveResult.EndOfMessage)
                            {
                                // We received data, but it is not complete, write it to a memory stream for reassembling
                                multiPartMessage = true;
                                if (memoryStream == null)
                                    memoryStream = new MemoryStream();
#if DEBUG
                                SocketLog?.Trace($"Socket {Id} received {receiveResult.Count} bytes in partial message");
#endif
                                await memoryStream.WriteAsync(buffer.Array, buffer.Offset, receiveResult.Count).ConfigureAwait(false);
                            }
                            else
                            {
                                if (!multiPartMessage)
                                {
                                    // Received a complete message and it's not multi part
#if DEBUG
                                    SocketLog?.Trace($"Socket {Id} received {receiveResult.Count} bytes in single message");
#endif
                                    HandleMessage(buffer.Array, buffer.Offset, receiveResult.Count, receiveResult.MessageType);
                                }
                                else
                                {
                                    // Received the end of a multipart message, write to memory stream for reassembling
#if DEBUG
                                    SocketLog?.Trace($"Socket {Id} received {receiveResult.Count} bytes in partial message");
#endif
                                    await memoryStream!.WriteAsync(buffer.Array, buffer.Offset, receiveResult.Count).ConfigureAwait(false);
                                }
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            _ = Task.Run(async () => await CloseInternalAsync(true, true).ConfigureAwait(false));
                            break;
                        }
                    }

                    lock (_receivedMessagesLock)
                        UpdateReceivedMessages();

                    if (receiveResult?.MessageType == WebSocketMessageType.Close)
                    {
                        // Received close message
                        break;
                    }

                    if (receiveResult == null || _closing)
                    {
                        // Error during receiving or cancellation requested, stop.
                        break;
                    }

                    if (multiPartMessage && receiveResult?.EndOfMessage == true)
                    {
                        // Reassemble complete message from memory stream
#if DEBUG
                        SocketLog?.Trace($"Socket {Id} reassembled message of {memoryStream!.Length} bytes");
#endif

                        HandleMessage(memoryStream!.ToArray(), 0, (int)memoryStream.Length, receiveResult.MessageType);
                        memoryStream.Dispose();
                    }
                }
            }
            catch
            {
                // No
            }
            finally
            {
                _startedReceive = false;
            }
        }

        /// <summary>
        /// Handles the message
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <param name="messageType"></param>
        private void HandleMessage(byte[] data, int offset, int count, WebSocketMessageType messageType)
        {
            try
            {
                string strData;

                if (messageType == WebSocketMessageType.Binary)
                {
                    if (DataInterpreterBytes == null)
                    {
                        return;
                    }

                    var relevantData = new byte[count];
                    Array.Copy(data, offset, relevantData, 0, count);
                    strData = DataInterpreterBytes(relevantData);
                }
                else
                {
                    strData = _encoding.GetString(data, offset, count);
                }
                if (DataInterpreterString != null)
                {
                    strData = DataInterpreterString(strData);
                }

                Handle(messageHandlers, strData);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Checks if there is no data received for a period longer than the specified timeout
        /// </summary>
        /// <returns></returns>
        protected async Task CheckTimeoutAsync()
        {
#if DEBUG
            SocketLog?.Debug($"Socket {Id} Starting task checking for no data received for {Timeout}");
#endif
            try
            {
                while (!_ctsSource.Token.IsCancellationRequested)
                {
                    if (_closing)
                        return;

                    if (DateTime.UtcNow - LastActionTime > Timeout)
                    {
#if DEBUG
                        SocketLog?.Warning($"Socket {Id} No data received for {Timeout}, reconnecting socket");
#endif
                        _ = CloseAsync().ConfigureAwait(false);
                        return;
                    }
                    try
                    {
                        await Task.Delay(500, _ctsSource.Token).ConfigureAwait(false);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Helper to invoke handlers
        /// </summary>
        /// <param name="handlers"></param>
        protected void Handle(List<Action> handlers)
        {
            LastActionTime = DateTime.UtcNow;
            foreach (var handle in new List<Action>(handlers))
                handle?.Invoke();
        }

        /// <summary>
        /// Helper to invoke handlers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handlers"></param>
        /// <param name="data"></param>
        protected void Handle<T>(List<Action<T>> handlers, T data)
        {
            LastActionTime = DateTime.UtcNow;
            foreach (var handle in new List<Action<T>>(handlers))
                handle?.Invoke(data);
        }

        /// <summary>
        /// Get the next identifier
        /// </summary>
        /// <returns></returns>
        private static int NextStreamId()
        {
            lock (streamIdLock)
            {
                lastStreamId++;
                return lastStreamId;
            }
        }

        /// <summary>
        /// Update the received messages list, removing messages received longer than 3s ago
        /// </summary>
        protected void UpdateReceivedMessages()
        {
            var checkTime = DateTime.UtcNow;
            if (checkTime - _lastReceivedMessagesUpdate > TimeSpan.FromSeconds(1))
            {
                foreach (var msg in _receivedMessages.ToList()) // To list here because we're removing from the list
                    if (checkTime - msg.Timestamp > TimeSpan.FromSeconds(3))
                        _receivedMessages.Remove(msg);

                _lastReceivedMessagesUpdate = checkTime;
            }
        }
    }

    /// <summary>
    /// Received message info
    /// </summary>
    public struct ReceiveItem
    {
        /// <summary>
        /// Timestamp of the received data
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Number of bytes received
        /// </summary>
        public int Bytes { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="bytes"></param>
        public ReceiveItem(DateTime timestamp, int bytes)
        {
            Timestamp = timestamp;
            Bytes = bytes;
        }
    }
}