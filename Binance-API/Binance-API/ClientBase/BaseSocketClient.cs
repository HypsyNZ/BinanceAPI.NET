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

using BinanceAPI.ClientHosts;
using BinanceAPI.Enums;
using BinanceAPI.Objects;
using BinanceAPI.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using static BinanceAPI.Logging;

namespace BinanceAPI.ClientBase
{
    /// <summary>
    /// The Base Socket Client
    /// <para>Created Automatically when you Subscribe to a SocketSubscription</para>
    /// <para>Wraps the Underlying ClientWebSocket</para>
    /// </summary>
    public class BaseSocketClient
    {
        /// <summary>
        /// Connectiion restored event
        /// </summary>
        public event Action<TimeSpan> ConnectionRestored;

        /// <summary>
        /// Occurs when the status of the socket changes
        /// </summary>
        public event Action<ConnectionStatus> ConnectionStatusChanged;

        /// <summary>
        /// Unhandled message event
        /// </summary>
        public event Action<JToken> UnhandledMessage;

        private SocketSubscription Subscription;
        private readonly object subscriptionLock = new();
        private readonly SocketClientHost socketClient;
        private readonly List<PendingRequest> pendingRequests;

        internal static int _lastStreamId;
        private static readonly object _streamIdLock = new();
        private volatile bool _resetting;

        private Task? _sendTask;
        private Task? _receiveTask;

        private ApiProxy? _proxy;
        private Encoding _encoding = Encoding.UTF8;
        private AsyncResetEvent _sendEvent;
        private ConcurrentQueue<byte[]> _sendBuffer;

        [AllowNull]
        private volatile MemoryStream memoryStream = null;

        [AllowNull]
        private volatile WebSocketReceiveResult receiveResult = null;

        private volatile bool multiPartMessage = false;
        private ArraySegment<byte> buffer = new();

        /// <summary>
        /// The Real Underlying Socket
        /// </summary>
        public ClientWebSocket ClientSocket { get; protected set; }

        /// <summary>
        /// The Current Status of the Socket Connection
        /// </summary>
        public ConnectionStatus SocketConnectionStatus { get; set; }

        /// <summary>
        /// If the socket should be reconnected upon closing
        /// </summary>
        public bool ShouldAttemptConnection { get; set; } = true;

        /// <summary>
        /// Time of disconnecting
        /// </summary>
        public DateTime? DisconnectTime { get; set; }

        /// <summary>
        /// The timestamp this socket has been active for the last time
        /// </summary>
        public DateTime LastActionTime { get; internal set; }

        /// <summary>
        /// The id of this socket
        /// </summary>
        public int Id { get; internal set; }

        /// <summary>
        /// Url this socket connects to
        /// </summary>
        public string Url { get; internal set; }

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
        /// The Base Client for handling a SocketSubscription
        /// </summary>
        /// <param name="url">The url the socket should connect to</param>
        /// <param name="client"></param>
        /// <param name="proxy"></param>
        internal BaseSocketClient(string url, SocketClientHost client, [AllowNull] ApiProxy proxy = null)
        {
            Id = NextStreamId();
            Url = url;

            if (proxy != null)
            {
                _proxy = proxy;
            }

            NewSocket();

            socketClient = client;

            pendingRequests = new List<PendingRequest>();

            DisconnectTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Add subscription to this connection
        /// </summary>
        public void AddSubscription(SocketSubscription subscription)
        {
            lock (subscriptionLock)
            {
                Subscription = subscription;
            }
        }

        /// <summary>
        /// Get the subscription on this connection
        /// </summary>
        public SocketSubscription? GetSubscription()
        {
            lock (subscriptionLock)
            {
                return Subscription;
            }
        }

        /// <summary>
        /// Send data and wait for an answer
        /// </summary>
        /// <typeparam name="T">The data type expected in response</typeparam>
        /// <param name="obj">The object to send</param>
        /// <param name="timeout">The timeout for response</param>
        /// <param name="handler">The response handler</param>
        /// <returns></returns>
        public Task SendAndWaitAsync<T>(T obj, TimeSpan timeout, Func<JToken, bool> handler)
        {
            var pending = new PendingRequest(handler, timeout);
            lock (pendingRequests)
            {
                pendingRequests.Add(pending);
            }

            Send(obj);
            return pending.Event.WaitAsync(timeout);
        }

        /// <summary>
        /// Send data over the websocket connection
        /// </summary>
        /// <typeparam name="T">The type of the object to send</typeparam>
        /// <param name="obj">The object to send</param>
        /// <param name="nullValueHandling">How null values should be serialized</param>
        public void Send<T>(T obj, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
        {
            if (obj is string str)
            {
                Send(str);
            }
            else
            {
                Send(JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings { NullValueHandling = nullValueHandling }));
            }
        }

        private void SocketOnStatusChanged(ConnectionStatus obj)
        {
            SocketConnectionStatus = obj;
            ConnectionStatusChanged?.Invoke(obj);
        }

        private void ProcessMessage(string data)
        {
            var timestamp = DateTime.UtcNow;
#if DEBUG
            SocketLog?.Trace($"Socket {Id} received data: " + data);
#endif
            if (string.IsNullOrEmpty(data)) return;

            var tokenData = data.ToJToken();
            if (tokenData == null)
            {
                data = $"\"{data}\"";
                tokenData = data.ToJToken();
                if (tokenData == null)
                    return;
            }

            // Message was not a request response, check data handlers
#if DEBUG
            var messageEvent = new MessageEvent(tokenData, Json.OutputOriginalData ? data : null, timestamp);
#else
            var messageEvent = new MessageEvent(tokenData, null, timestamp);
#endif
            if (!HandleData(messageEvent))
            {
                PendingRequest[] requests;
                lock (pendingRequests)
                {
                    requests = pendingRequests.ToArray();
                }

                // Check if this message is an answer on any pending requests
                foreach (var request in requests)
                {
                    if (request.Completed)
                    {
                        lock (pendingRequests)
                        {
                            pendingRequests.Remove(request);
                        }
                    }
                    else
                    if (request.CheckData(tokenData))
                    {
                        lock (pendingRequests)
                        {
                            pendingRequests.Remove(request);
                        }

                        return;
                    }
                }

                SocketLog?.Error($"Socket {Id} Message not handled: " + tokenData.ToString());
                UnhandledMessage?.Invoke(tokenData);
            }
        }

        private async Task ConnectionAttemptLoop(bool connecting = false)
        {
            if (SocketConnectionStatus == ConnectionStatus.Connecting || SocketConnectionStatus == ConnectionStatus.Waiting)
            {
                return;
            }

            while (true)
            {
                if (!ShouldAttemptConnection)
                {
                    await DisposeAsync().ConfigureAwait(false);
                    return;
                }

                await Task.Delay(1).ConfigureAwait(false);

                bool completed = await ConnectionAttempt(connecting).ConfigureAwait(false);
                if (completed)
                {
                    return;
                }
            }

        }

        private async Task<bool> ConnectionAttempt(bool connecting = false)
        {
            var ReconnectTry = 0;
            var ResubscribeTry = 0;

            // Connection
            while (ShouldAttemptConnection)
            {
                SocketOnStatusChanged(ConnectionStatus.Waiting);
                await Task.Delay(1984).ConfigureAwait(false);
                SocketOnStatusChanged(ConnectionStatus.Connecting);

                ReconnectTry++;
                NewSocket(true);
                if (!await ConnectAsync().ConfigureAwait(false))
                {
                    if (ReconnectTry >= socketClient.MaxReconnectTries)
                    {
                        SocketLog?.Debug($"Socket {Id} failed to {(connecting ? "connect" : "reconnect")} after {ReconnectTry} tries, closing");
                        await DisposeAsync().ConfigureAwait(false);
                        return true;
                    }
                    else
                    {
                        SocketLog?.Debug($"[{DateTime.UtcNow - DisconnectTime}] Socket [{Id}]  Failed to {(connecting ? "Connect" : "Reconnect")} - Attempts: {ReconnectTry}/{socketClient.MaxReconnectTries}");
                    }
                }
                else
                {
                    SocketLog?.Info($"[{DateTime.UtcNow - DisconnectTime}] Socket [{Id}] {(connecting ? "Connected" : "Reconnected")} - Attempts: {ReconnectTry}/{socketClient.MaxReconnectTries}");
                    break;
                }
            }

            // Subscription
            while (ShouldAttemptConnection)
            {
                ResubscribeTry++;
                var reconnectResult = await ProcessResubscriptionAsync().ConfigureAwait(false);
                if (!reconnectResult)
                {
                    if (ResubscribeTry >= socketClient.MaxReconnectTries)
                    {
                        SocketLog?.Debug($"Socket {Id} failed to {(connecting ? "subscribe" : "resubscribe")} after {ResubscribeTry} tries, closing");
                        await DisposeAsync().ConfigureAwait(false);
                        return true;
                    }
                    else
                    {
                        SocketLog?.Debug($"Socket {Id}  {(connecting ? "subscribing" : "resubscribing")} subscription on  {(connecting ? "connected" : "reconnected")} socket{(socketClient.MaxReconnectTries != null ? $", try {ResubscribeTry}/{socketClient.MaxReconnectTries}" : "")}..");
                    }

                    if (!IsOpen)
                    {
                        // Disconnected while resubscribing
                        return false;
                    }
                }
                else
                {
                    ReconnectTry = 0;
                    ResubscribeTry = 0;
                    _ = Task.Run(() => ConnectionRestored?.Invoke(DisconnectTime.HasValue ? DateTime.UtcNow - DisconnectTime.Value : TimeSpan.FromSeconds(0))).ConfigureAwait(false);
                    return true;
                }
            }

            if (!ShouldAttemptConnection)
            {
                await DisposeAsync().ConfigureAwait(false);
            }

            return true;
        }

        private bool HandleData(MessageEvent messageEvent)
        {
            try
            {
                lock (subscriptionLock)
                {
                    if (socketClient.MessageMatchesHandler(messageEvent.JsonData, Subscription!.Request))
                    {
                        messageEvent.JsonData = (JToken)messageEvent.JsonData;
                        Subscription.MessageHandler(messageEvent);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                SocketLog?.Error($"Socket {Id} Exception during message processing Data: {messageEvent.JsonData}", ex);
            }

            return false;
        }

        private async Task<bool> ProcessResubscriptionAsync()
        {
            if (!IsOpen)
            {
                return false;
            }

            var task = await socketClient.SubscribeAndWaitAsync(this, Subscription!.Request!, Subscription).ConfigureAwait(false);

            if (!task.Success || !IsOpen)
            {
                return false;
            }

            return true;
        }

        internal async Task UnsubscribeAsync(SocketSubscription socketSubscription)
        {
            await socketClient.UnsubscribeAsync(this, socketSubscription).ConfigureAwait(false);
        }

        internal async Task<CallResult<bool>> ResubscribeAsync(SocketSubscription socketSubscription)
        {
            if (!IsOpen)
            {
                return new CallResult<bool>(false, new UnknownError("Socket is not connected"));
            }

            return await socketClient.SubscribeAndWaitAsync(this, socketSubscription.Request!, socketSubscription).ConfigureAwait(false);
        }

        /// <summary>
        /// Closes the subscription on this connection
        /// <para>Closes the connection if the correct subscription is provided</para>
        /// </summary>
        /// <param name="subscription">Subscription to close</param>
        /// <returns></returns>
        public async Task CloseAndDisposeSubscriptionAsync(SocketSubscription subscription)
        {
            if (!IsOpen)
            {
                return;
            }

            if (subscription.Confirmed)
            {
                var result = await socketClient.UnsubscribeAsync(this, subscription).ConfigureAwait(false);
                if (result)
                {
                    SocketLog?.Debug($"Socket {Id} subscription successfully unsubscribed, Closing connection..");
                }
                else
                {
                    SocketLog?.Debug($"Socket {Id} subscription seems to already be unsubscribed, Closing connection..");
                }

                await DisposeAsync().ConfigureAwait(false);
            }
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

            _sendEvent = new AsyncResetEvent();
            _sendBuffer = new ConcurrentQueue<byte[]>();

            ClientSocket = new ClientWebSocket();

            if (_proxy != null)
            {
                SetProxy(_proxy);
            }

            ClientSocket.Options.KeepAliveInterval = TimeSpan.FromMinutes(1);
            ClientSocket.Options.SetBuffer(8192, 8192);

            _resetting = false;
        }

        /// <summary>
        /// Set a proxy to use. Should be set before connecting
        /// </summary>
        /// <param name="proxy"></param>
        public void SetProxy(ApiProxy proxy)
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
        public async Task<bool> ConnectAsync()
        {
#if DEBUG
            Logging.SocketLog?.Debug($"Socket {Id} connecting..");
#endif
            try
            {
                await ClientSocket.ConnectAsync(UriClient.GetStream(), default).ConfigureAwait(false);
#if DEBUG
                Logging.SocketLog?.Trace($"Socket {Id} connection succeeded, starting communication..");
#endif
                _sendTask = Task.Factory.StartNew(SendLoopAsync, default, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();
                _receiveTask = Task.Factory.StartNew(DigestLoop, default, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default).Unwrap();

#if DEBUG
                Logging.SocketLog?.Debug($"Socket {Id} connected..");
#endif
                SocketOnStatusChanged(ConnectionStatus.Connected);
                SocketLog?.Debug($"Socket {Id} connected to {Url}");
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
                SocketOnStatusChanged(ConnectionStatus.Error);
                return false;
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
        /// Loop for sending data
        /// </summary>
        /// <returns></returns>
        private async Task SendLoopAsync()
        {
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
                await InternalResetSocketAsync().ConfigureAwait(false);
            }
        }

        private async Task DigestLoop()
        {
            while (!_resetting)
            {
                ReceiveLoopAsync();
            }
        }

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
                        await InternalResetSocketAsync().ConfigureAwait(false);
                        return;
                    }

                    if (receiveResult.EndOfMessage && !multiPartMessage)
                    {
                        ProcessMessage(Encoding.GetString(buffer.Array, buffer.Offset, receiveResult.Count));
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
                        ProcessMessage(Encoding.GetString(memoryStream!.ToArray(), 0, (int)memoryStream.Length));
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
                await InternalResetSocketAsync().ConfigureAwait(false);
            }
        }

        private void Handle(List<Action> handlers)
        {
            LastActionTime = DateTime.UtcNow;
            foreach (var handle in handlers.ToArray())
            {
                handle?.Invoke();
            }
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
        /// Connect the Socket
        /// </summary>
        /// <returns></returns>
        internal async Task InternalConnectSocketAsync()
        {
            if (SocketConnectionStatus == ConnectionStatus.Connecting || SocketConnectionStatus == ConnectionStatus.Waiting || _resetting)
            {
                return;
            }

            _resetting = true;

            Logging.SocketLog?.Debug($"Connecting Socket {Id}..");
            _ = Task.Run(async () =>
            {
                await ConnectionAttemptLoop(true).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Internal reset method, Will prepare the socket to be reset so it can be automatically reconnected or closed permanantly
        /// </summary>
        /// <returns></returns>
        public async Task InternalResetSocketAsync(bool connectAttempt = false)
        {
            if (SocketConnectionStatus == ConnectionStatus.Connecting || SocketConnectionStatus == ConnectionStatus.Waiting || _resetting)
            {
                return;
            }

            _resetting = true;
            _sendEvent.Set();

            if (ClientSocket.State == WebSocketState.Open)
            {
                await ClientSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, default).ConfigureAwait(false);
                Logging.SocketLog?.Debug($"Socket {Id} has closed..");
            }
            else
            {
                Logging.SocketLog?.Debug($"Socket {Id} was already closed..");
            }

            SocketOnStatusChanged(ConnectionStatus.Disconnected);

            lock (pendingRequests)
            {
                foreach (var pendingRequest in pendingRequests.ToList())
                {
                    pendingRequest.Fail();
                    pendingRequests.Remove(pendingRequest);
                }
            }

            if (ShouldAttemptConnection)
            {
                if (SocketConnectionStatus == ConnectionStatus.Connecting || SocketConnectionStatus == ConnectionStatus.Waiting)
                {
                    return; // Already reconnecting
                }

                DisconnectTime = DateTime.UtcNow;
                SocketOnStatusChanged(ConnectionStatus.Lost);
                SocketLog?.Info($"Socket {Id} Connection lost, will try to reconnect after 2000ms");

                _ = Task.Run(async () =>
                {
                    await ConnectionAttemptLoop().ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
            else
            {
                SocketOnStatusChanged(ConnectionStatus.Closed);
                await DisposeAsync().ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Close and Dispose the Socket and all Message Handlers
        /// </summary>
        public async Task DisposeAsync()
        {
            SocketLog?.Info($"Socket {Id} closed");
            ShouldAttemptConnection = false;

            await InternalResetSocketAsync().ConfigureAwait(false);

            if (socketClient.AllSockets.ContainsKey(Id))
            {
                socketClient.AllSockets.TryRemove(Id, out _);
            }

            DisconnectTime = DateTime.UtcNow;

            ClientSocket.Dispose();
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
