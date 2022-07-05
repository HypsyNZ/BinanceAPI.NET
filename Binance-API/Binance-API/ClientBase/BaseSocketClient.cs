/*
*MIT License
*
*Copyright (c) 2022 S Christison
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/

using BinanceAPI.Enums;
using BinanceAPI.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.Clients
{
    /// <summary>
    /// A wrapper around System.Net.WebSockets.ClientWebSocket
    /// </summary>
    public class BaseSocketClient
    {
        /// <summary>
        /// The Real Underlying Socket
        /// </summary>
        public ClientWebSocket ClientSocket { get; protected set; }

        /// <summary>
        /// Socket closed event
        /// </summary>
        public event Action OnClose
        {
            add => closeHandlers.Add(value);
            remove => closeHandlers.Remove(value);
        }

        /// <summary>
        /// Socket connect event
        /// </summary>
        public event Action OnConnect
        {
            add => connectHandlers.Add(value);
            remove => connectHandlers.Remove(value);
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
        /// Occurs when the status of the socket changes
        /// </summary>
        public event Action<ConnectionStatus>? StatusChanged;

        internal static int _lastStreamId;
        private static readonly object _streamIdLock = new();

        private Task? _sendTask;
        private Task? _receiveTask;

        private Encoding _encoding = Encoding.UTF8;

        private AsyncResetEvent _sendEvent;
        private ConcurrentQueue<byte[]> _sendBuffer;

        private volatile bool _resetting;
        private volatile bool _startedSent;
        private volatile bool _startedReceive;

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
        /// Handlers for when the the connecting is opening
        /// </summary>
        protected readonly List<Action> connectHandlers = new();

        /// <summary>
        /// Handlers for when a message is received
        /// </summary>
        protected readonly List<Action<string>> messageHandlers = new();

        /// <summary>
        /// The id of this socket
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// The timestamp this socket has been active for the last time
        /// </summary>
        public DateTime LastActionTime { get; internal set; }

        /// <summary>
        /// Url this socket connects to
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// If the connection is closed
        /// </summary>
        public bool IsClosed => ClientSocket.State == WebSocketState.Closed;

        /// <summary>
        /// If the connection is closing
        /// </summary>
        public bool IsClosing => _resetting;

        /// <summary>
        /// If the connection is open
        /// </summary>
        public bool IsOpen => ClientSocket.State == WebSocketState.Open && !IsClosing;

        /// <summary>
        /// Encoding used for decoding the received bytes into a string
        /// </summary>
        public Encoding Encoding
        {
            get => _encoding;
            set
            {
                _encoding = value;
            }
        }

        /// <summary>
        /// The Client for this Socket
        /// </summary>
        /// <param name="url">The url the socket should connect to</param>
        public BaseSocketClient(string url)
        {
            Id = NextStreamId();
            Url = url;
            NewSocket();
        }

        /// <summary>
        /// Create the socket object
        /// </summary>
        //[InConstructor]
        public void NewSocket(bool reset = false)
        {
            if (reset)
            {
                Logging.SocketLog?.Debug($"Socket {Id} resetting..");
            }

            _startedReceive = false;
            _startedSent = false;

            _sendEvent = new AsyncResetEvent();
            _sendBuffer = new ConcurrentQueue<byte[]>();

            ClientSocket = new ClientWebSocket();
            ClientSocket.Options.KeepAliveInterval = TimeSpan.FromMinutes(1);
            ClientSocket.Options.SetBuffer(8192, 8192);

            _resetting = false;
        }

        /// <summary>
        /// Set a proxy to use. Should be set before connecting
        /// </summary>
        /// <param name="proxy"></param>
        public virtual void SetProxy(ApiProxy proxy)
        {
            Uri.TryCreate($"{proxy.Host}:{proxy.Port}", UriKind.Absolute, out var uri);
            ClientSocket.Options.Proxy = uri?.Scheme == null
                ? ClientSocket.Options.Proxy = new WebProxy(proxy.Host, proxy.Port)
                : ClientSocket.Options.Proxy = new WebProxy
                {
                    Address = uri
                };

            if (proxy.Login != null)
            {
                ClientSocket.Options.Proxy.Credentials = new NetworkCredential(proxy.Login, proxy.Password);
            }
        }

        /// <summary>
        /// Connect the websocket
        /// </summary>
        /// <returns>True if successfull</returns>
        public virtual async Task<bool> ConnectAsync()
        {
#if DEBUG
            Logging.SocketLog?.Debug($"Socket {Id} connecting..");
#endif
            StatusChanged?.Invoke(ConnectionStatus.Connecting);
            try
            {
                await ClientSocket.ConnectAsync(new Uri(Url), default).ConfigureAwait(false);
                Handle(openHandlers);
#if DEBUG
                Logging.SocketLog?.Trace($"Socket {Id} connection succeeded, starting communication..");
#endif
                _sendTask = Task.Factory.StartNew(SendLoopAsync, default, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
                _receiveTask = Task.Factory.StartNew(DigestLoop, default, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();

                var sw = Stopwatch.StartNew();
                while (!_startedSent || !_startedReceive)
                {
                    // Wait for the tasks to have actually started
                    await Task.Delay(1).ConfigureAwait(false);

                    if (sw.ElapsedMilliseconds > 5000)
                    {
                        _ = ClientSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", default);
#if DEBUG
                        Logging.SocketLog?.Debug($"Socket {Id} startup interupted..");
#endif
                        StatusChanged?.Invoke(ConnectionStatus.Disconnected);
                        return false;
                    }
                }
#if DEBUG
                Logging.SocketLog?.Debug($"Socket {Id} connected..");
#endif
                return true;
            }
            catch
#if DEBUG
            (Exception e)
#endif
            {
#if DEBUG
                Logging.SocketLog?.Debug($"Socket {Id} connection failed: " + e.ToLogString());
#endif
                StatusChanged?.Invoke(ConnectionStatus.Error);
                return false;
            }
        }

        /// <summary>
        /// Internal reset method, Will prepare the socket to be reset so it can be automatically reconnected or closed permanantly
        /// </summary>
        /// <returns></returns>
        public async Task InternalResetAsync(bool connectAttempt = false)
        {
            if (_resetting)
            {
                return;
            }

            _resetting = true;
            _sendEvent.Set();

            if (connectAttempt)
            {
                Logging.SocketLog?.Debug($"Connecting Socket {Id}..");
                Handle(connectHandlers);
            }
            else
            {
                if (ClientSocket.State == WebSocketState.Open)
                {
                    await ClientSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, default).ConfigureAwait(false);
                    StatusChanged?.Invoke(ConnectionStatus.Disconnected);
                    Logging.SocketLog?.Debug($"Socket {Id} has closed..");
                }
                else
                {
                    Logging.SocketLog?.Debug($"Socket {Id} was already closed..");
                }

                Handle(closeHandlers);
            }
        }

        /// <summary>
        /// Send data over the websocket
        /// </summary>
        /// <param name="data">Data to send</param>
        public void Send(string data)
        {
            if (_resetting)
            {
                return;
            }

            var bytes = Encoding.GetBytes(data);
#if DEBUG
            Logging.SocketLog?.Trace($"Socket {Id} Adding {bytes.Length} to sent buffer..");
#endif
            _sendBuffer.Enqueue(bytes);
            _sendEvent.Set();
        }

        /// <summary>
        /// Loop for sending dataW
        /// </summary>
        /// <returns></returns>
        private async Task SendLoopAsync()
        {
            _startedSent = true;
            try
            {
                while (!_resetting)
                {
                    await _sendEvent.WaitAsync().ConfigureAwait(false);

                    while (!_resetting)
                    {
                        bool s = _sendBuffer.TryDequeue(out var data);
                        if (s)
                        {
                            await ClientSocket.SendAsync(new ArraySegment<byte>(data, 0, data.Length), WebSocketMessageType.Text, true, default).ConfigureAwait(false);
#if DEBUG
                            Logging.SocketLog?.Trace($"Socket {Id} sent {data.Length} bytes..");
#endif
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                Logging.SocketLog?.Error(ex);
            }
            finally
            {
                await InternalResetAsync().ConfigureAwait(false);
            }
        }

        private async Task DigestLoop()
        {
            _startedReceive = true;
            while (!_resetting)
            {
                ReceiveLoopAsync();
            }
        }

        private volatile MemoryStream? memoryStream = null;
        private volatile WebSocketReceiveResult? receiveResult = null;
        private volatile bool multiPartMessage = false;
        private ArraySegment<byte> buffer = new();

        /// <summary>
        /// Loop for receiving and reassembling data
        /// </summary>
        /// <returns></returns>
        private async void ReceiveLoopAsync()
        {
            try
            {
                buffer = new ArraySegment<byte>(new byte[8192]);
                multiPartMessage = false;
                memoryStream = null;
                receiveResult = null;

                while (!_resetting)
                {
                    receiveResult = ClientSocket.ReceiveAsync(buffer, default).Result;

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        Logging.SocketLog?.Debug($"Socket {Id} received `Close` message..");
                        await InternalResetAsync().ConfigureAwait(false);
                        return;
                    }

                    if (receiveResult.EndOfMessage && !multiPartMessage)
                    {
                        Handle(messageHandlers, Encoding.GetString(buffer.Array, buffer.Offset, receiveResult.Count));
                        return;
                    }

                    if (!receiveResult.EndOfMessage)
                    {
                        memoryStream ??= new MemoryStream();

                        await memoryStream.WriteAsync(buffer.Array, buffer.Offset, receiveResult.Count).ConfigureAwait(false);

                        multiPartMessage = true;
#if DEBUG
                        Logging.SocketLog?.Trace($"Socket {Id} received {receiveResult.Count} bytes in partial message..");
#endif
                    }
                    else
                    {
                        await memoryStream!.WriteAsync(buffer.Array, buffer.Offset, receiveResult.Count).ConfigureAwait(false);
                        Handle(messageHandlers, Encoding.GetString(memoryStream!.ToArray(), 0, (int)memoryStream.Length));
#if DEBUG
                        Logging.SocketLog?.Trace($"Socket {Id} reassembled message of {memoryStream!.Length} bytes | partial message count: {receiveResult.Count}");
#endif
                        memoryStream.Dispose();
                        return;
                    }
                }
            }
            catch
            {
                // Okay buddy
            }
        }

        private void Handle(List<Action> handlers)
        {
            LastActionTime = DateTime.UtcNow;
            foreach (var handle in handlers.ToArray())
                handle?.Invoke();
        }

        private void Handle(List<Action<string>> handlers, string data)
        {
            LastActionTime = DateTime.UtcNow;
            foreach (var handle in handlers.ToArray())
                handle?.Invoke(data);
        }

        private static int NextStreamId()
        {
            lock (_streamIdLock)
            {
                _lastStreamId++;
                return _lastStreamId;
            }
        }

        /// <summary>
        /// Dispose the Socket and all Message Handlers
        /// </summary>
        public void Dispose()
        {
            ClientSocket.Dispose();
            errorHandlers.Clear();
            openHandlers.Clear();
            closeHandlers.Clear();
            messageHandlers.Clear();

#if DEBUG
            Logging.SocketLog?.Trace($"Socket {Id} disposed..");
#endif
        }

        /// <inheritdoc/>
        public SslProtocols SSLProtocols
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public double IncomingKbps => throw new NotImplementedException();
    }
}
