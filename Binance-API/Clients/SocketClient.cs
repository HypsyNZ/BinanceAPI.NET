using BinanceAPI.Authentication;
using BinanceAPI.Interfaces;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Other;
using BinanceAPI.Options;
using BinanceAPI.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using static BinanceAPI.Logging;

namespace BinanceAPI
{
    /// <summary>
    /// Base for socket client implementations
    /// </summary>
    public abstract class SocketClient : BaseClient, ISocketClient
    {
        #region fields

        /// <summary>
        /// The factory for creating sockets. Used for unit testing
        /// </summary>
        public IWebsocketFactory SocketFactory { get; set; } = new WebsocketFactory();

        /// <summary>
        /// List of socket connections currently connecting/connected
        /// </summary>
        protected internal ConcurrentDictionary<int, SocketConnection> sockets = new();

        /// <summary>
        /// Semaphore used while creating sockets
        /// </summary>
        protected internal readonly SemaphoreSlim semaphoreSlim = new(1);

        /// <inheritdoc cref="SocketClientOptions.ReconnectInterval"/>
        public TimeSpan ReconnectInterval { get; }

        /// <inheritdoc cref="SocketClientOptions.AutoReconnect"/>
        public bool AutoReconnect { get; }

        /// <inheritdoc cref="SocketClientOptions.SocketResponseTimeout"/>
        public TimeSpan ResponseTimeout { get; }

        /// <inheritdoc cref="SocketClientOptions.SocketNoDataTimeout"/>
        public TimeSpan SocketNoDataTimeout { get; }

        /// <summary>
        /// The max amount of concurrent socket connections
        /// </summary>
        public int MaxSocketConnections { get; protected set; } = 9999;

        /// <inheritdoc cref="SocketClientOptions.SocketSubscriptionsCombineTarget"/>
        public int SocketCombineTarget { get; protected set; }

        /// <inheritdoc cref="SocketClientOptions.MaxReconnectTries"/>
        public int? MaxReconnectTries { get; protected set; }

        /// <inheritdoc cref="SocketClientOptions.MaxResubscribeTries"/>
        public int? MaxResubscribeTries { get; protected set; }

        /// <inheritdoc cref="SocketClientOptions.MaxConcurrentResubscriptionsPerSocket"/>
        public int MaxConcurrentResubscriptionsPerSocket { get; protected set; }

        /// <summary>
        /// Delegate used for processing byte data received from socket connections before it is processed by handlers
        /// </summary>
        protected Func<byte[], string>? dataInterpreterBytes;

        /// <summary>
        /// Delegate used for processing string data received from socket connections before it is processed by handlers
        /// </summary>
        protected Func<string, string>? dataInterpreterString;

        /// <summary>
        /// Handlers for data from the socket which doesn't need to be forwarded to the caller. Ping or welcome messages for example.
        /// </summary>
        protected Dictionary<string, Action<MessageEvent>> genericHandlers = new();

        /// <summary>
        /// The task that is sending periodic data on the websocket. Can be used for sending Ping messages every x seconds or similair. Not necesarry.
        /// </summary>
        protected Task? periodicTask;

        /// <summary>
        /// Wait event for the periodicTask
        /// </summary>
        protected AsyncResetEvent? periodicEvent;

        /// <summary>
        /// If client is disposing
        /// </summary>
        protected bool disposing;

        /// <summary>
        /// If true; data which is a response to a query will also be distributed to subscriptions
        /// If false; data which is a response to a query won't get forwarded to subscriptions as well
        /// </summary>
        protected internal bool ContinueOnQueryResponse { get; protected set; }

        /// <summary>
        /// If a message is received on the socket which is not handled by a handler this boolean determines whether this logs an error message
        /// </summary>
        protected internal bool UnhandledMessageExpected { get; set; }

        /// <summary>
        /// The current kilobytes per second of data being received by all connection from this client, averaged over the last 3 seconds
        /// </summary>
        public double IncomingKbps
        {
            get
            {
                if (!sockets.Any())
                    return 0;

                return sockets.Sum(s => s.Value.Socket.IncomingKbps);
            }
        }

        #endregion fields

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="exchangeName">The name of the exchange this client is for</param>
        /// <param name="exchangeOptions">The options for this client</param>
        /// <param name="authenticationProvider">The authentication provider for this client (can be null if no credentials are provided)</param>
        protected SocketClient(string exchangeName, SocketClientOptions exchangeOptions, AuthenticationProvider? authenticationProvider) : base(exchangeName, exchangeOptions, authenticationProvider)
        {
            if (exchangeOptions == null)
                throw new ArgumentNullException(nameof(exchangeOptions));

            AutoReconnect = exchangeOptions.AutoReconnect;
            ReconnectInterval = exchangeOptions.ReconnectInterval;
            ResponseTimeout = exchangeOptions.SocketResponseTimeout;
            SocketNoDataTimeout = exchangeOptions.SocketNoDataTimeout;
            SocketCombineTarget = exchangeOptions.SocketSubscriptionsCombineTarget ?? 1;
            MaxReconnectTries = exchangeOptions.MaxReconnectTries;
            MaxResubscribeTries = exchangeOptions.MaxResubscribeTries;
            MaxConcurrentResubscriptionsPerSocket = exchangeOptions.MaxConcurrentResubscriptionsPerSocket;

            Logging.StartSocketLog(exchangeOptions.LogPath, exchangeOptions.LogLevel);
        }

        /// <summary>
        /// Set a delegate to be used for processing data received from socket connections before it is processed by handlers
        /// </summary>
        /// <param name="byteHandler">Handler for byte data</param>
        /// <param name="stringHandler">Handler for string data</param>
        protected void SetDataInterpreter(Func<byte[], string>? byteHandler, Func<string, string>? stringHandler)
        {
            dataInterpreterBytes = byteHandler;
            dataInterpreterString = stringHandler;
        }

        /// <summary>
        /// Connect to an url and listen for data
        /// </summary>
        /// <typeparam name="T">The type of the expected data</typeparam>
        /// <param name="url">The URL to connect to</param>
        /// <param name="request">The optional request object to send, will be serialized to json</param>
        /// <param name="identifier">The identifier to use, necessary if no request object is sent</param>
        /// <param name="authenticated">If the subscription is to an authenticated endpoint</param>
        /// <param name="dataHandler">The handler of update data</param>
        /// <returns></returns>
        protected virtual async Task<CallResult<UpdateSubscription>> SubscribeAsync<T>(string url, object? request, string? identifier, bool authenticated, Action<DataEvent<T>> dataHandler)
        {
            SocketConnection socketConnection;
            SocketSubscription subscription;
            var released = false;
            // Wait for a semaphore here, so we only connect 1 socket at a time.
            // This is necessary for being able to see if connections can be combined
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            try
            {
                // Get a new or existing socket connection
                socketConnection = GetSocketConnection(url, authenticated);

                // Add a subscription on the socket connection
                subscription = AddSubscription(request, identifier, true, socketConnection, dataHandler);
                if (SocketCombineTarget == 1)
                {
                    // Only 1 subscription per connection, so no need to wait for connection since a new subscription will create a new connection anyway
                    semaphoreSlim.Release();
                    released = true;
                }

                var needsConnecting = !socketConnection.Connected;

                var connectResult = await ConnectIfNeededAsync(socketConnection, authenticated).ConfigureAwait(false);
                if (!connectResult)
                    return new CallResult<UpdateSubscription>(null, connectResult.Error);
#if DEBUG
                if (needsConnecting)
                    SocketLog?.Debug($"Socket {socketConnection.Socket.Id} connected to {url} {(request == null ? "" : "with request " + JsonConvert.SerializeObject(request))}");
#endif
            }
            finally
            {
                if (!released)
                    semaphoreSlim.Release();
            }

            if (socketConnection.PausedActivity)
            {
#if DEBUG
                SocketLog?.Info($"Socket {socketConnection.Socket.Id} has been paused, can't subscribe at this moment");
#endif
                return new CallResult<UpdateSubscription>(default, new ServerError("Socket is paused"));
            }

            if (request != null)
            {
                // Send the request and wait for answer
                var subResult = await SubscribeAndWaitAsync(socketConnection, request, subscription).ConfigureAwait(false);
                if (!subResult)
                {
                    await socketConnection.CloseAsync(subscription).ConfigureAwait(false);
                    return new CallResult<UpdateSubscription>(null, subResult.Error);
                }
            }
            else
            {
                // No request to be sent, so just mark the subscription as comfirmed
                subscription.Confirmed = true;
            }

            socketConnection.ShouldReconnect = true;
            return new CallResult<UpdateSubscription>(new UpdateSubscription(socketConnection, subscription), null);
        }

        /// <summary>
        /// Sends the subscribe request and waits for a response to that request
        /// </summary>
        /// <param name="socketConnection">The connection to send the request on</param>
        /// <param name="request">The request to send, will be serialized to json</param>
        /// <param name="subscription">The subscription the request is for</param>
        /// <returns></returns>
        protected internal virtual async Task<CallResult<bool>> SubscribeAndWaitAsync(SocketConnection socketConnection, object request, SocketSubscription subscription)
        {
            CallResult<object>? callResult = null;
            await socketConnection.SendAndWaitAsync(request, ResponseTimeout, data => HandleSubscriptionResponse(request, data, out callResult)).ConfigureAwait(false);

            if (callResult?.Success == true)
                subscription.Confirmed = true;

            return new CallResult<bool>(callResult?.Success ?? false, callResult == null ? new ServerError("No response on subscription request received") : callResult.Error);
        }

        /// <summary>
        /// Checks if a socket needs to be connected and does so if needed. Also authenticates on the socket if needed
        /// </summary>
        /// <param name="socket">The connection to check</param>
        /// <param name="authenticated">Whether the socket should authenticated</param>
        /// <returns></returns>
        protected virtual async Task<CallResult<bool>> ConnectIfNeededAsync(SocketConnection socket, bool authenticated)
        {
            if (socket.Connected)
                return new CallResult<bool>(true, null);

            var connectResult = await ConnectSocketAsync(socket).ConfigureAwait(false);
            if (!connectResult)
                return new CallResult<bool>(false, connectResult.Error);

            return new CallResult<bool>(true, null);
        }

        /// <summary>
        /// The socketConnection received data (the data JToken parameter). The implementation of this method should check if the received data is a response to the subscription request that was send (the request parameter).
        /// For example; A subscribe request message is send with an Id parameter with value 10. The socket receives data and calls this method to see if the data it received is an
        /// anwser to any subscription request that was done. The implementation of this method should check if the response.Id == request.Id to see if they match (assuming the api has some sort of Id tracking on messages,
        /// if not some other method has be implemented to match the messages).
        /// If the messages match, the callResult out parameter should be set with the deserialized data in the from of (T) and return true.
        /// </summary>
        /// <param name="request">The request that the subscription sent</param>
        /// <param name="message">JToken</param>
        /// <param name="callResult">The interpretation (null if message wasn't a response to the request)</param>
        /// <returns>True if the message was a response to the subscription request</returns>
        public bool HandleSubscriptionResponse(object request, JToken message, out CallResult<object>? callResult)
        {
            callResult = null;
            if (message.Type != JTokenType.Object)
                return false;

            var id = message["id"];
            if (id == null)
                return false;

            var bRequest = (BinanceSocketRequest)request;
            if ((int)id != bRequest.Id)
                return false;

            var result = message["result"];
            if (result != null && result.Type == JTokenType.Null)
            {
                callResult = new CallResult<object>(null, null);
                return true;
            }

            var error = message["error"];
            if (error == null)
            {
                callResult = new CallResult<object>(null, new ServerError("Unknown error: " + message.ToString()));
                return true;
            }

            callResult = new CallResult<object>(null, new ServerError(error["code"]!.Value<int>(), error["msg"]!.ToString()));
            return true;
        }

        /// <summary>
        /// Needs to check if a received message matches a handler by request. After subscribing data message will come in. These data messages need to be matched to a specific connection
        /// to pass the correct data to the correct handler. The implementation of this method should check if the message received matches the subscribe request that was sent.
        /// </summary>
        /// <param name="message">The received data</param>
        /// <param name="request">The subscription request</param>
        /// <returns>True if the message is for the subscription which sent the request</returns>
        public bool MessageMatchesHandler(JToken message, object request)
        {
            if (message.Type != JTokenType.Object)
                return false;

            var bRequest = (BinanceSocketRequest)request;
            var stream = message["stream"];
            if (stream == null)
                return false;

            return bRequest.Params.Contains(stream.ToString());
        }

        /// <summary>
        /// Needs to unsubscribe a subscription, typically by sending an unsubscribe request. If multiple subscriptions per socket is not allowed this can just return since the socket will be closed anyway
        /// </summary>
        /// <param name="connection">The connection on which to unsubscribe</param>
        /// <param name="subscription">The subscription to unsubscribe</param>
        /// <returns></returns>
        public async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscription)
        {
            var topics = ((BinanceSocketRequest)subscription.Request!).Params;
            var unsub = new BinanceSocketRequest { Method = "UNSUBSCRIBE", Params = topics, Id = NextId() };
            var result = false;

            if (!connection.Socket.IsOpen)
                return true;

            await connection.SendAndWaitAsync(unsub, ResponseTimeout, data =>
            {
                if (data.Type != JTokenType.Object)
                    return false;

                var id = data["id"];
                if (id == null)
                    return false;

                if ((int)id != unsub.Id)
                    return false;

                var result = data["result"];
                if (result?.Type == JTokenType.Null)
                {
                    result = true;
                    return true;
                }

                return true;
            }).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// Optional handler to interpolate data before sending it to the handlers
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected internal virtual JToken ProcessTokenData(JToken message)
        {
            return message;
        }

        /// <summary>
        /// Add a subscription to a connection
        /// </summary>
        /// <typeparam name="T">The type of data the subscription expects</typeparam>
        /// <param name="request">The request of the subscription</param>
        /// <param name="identifier">The identifier of the subscription (can be null if request param is used)</param>
        /// <param name="userSubscription">Whether or not this is a user subscription (counts towards the max amount of handlers on a socket)</param>
        /// <param name="connection">The socket connection the handler is on</param>
        /// <param name="dataHandler">The handler of the data received</param>
        /// <returns></returns>
        protected virtual SocketSubscription AddSubscription<T>(object? request, string? identifier, bool userSubscription, SocketConnection connection, Action<DataEvent<T>> dataHandler)
        {
            void InternalHandler(MessageEvent messageEvent)
            {
                if (typeof(T) == typeof(string))
                {
                    var stringData = (T)Convert.ChangeType(messageEvent.JsonData.ToString(), typeof(T));
#if DEBUG
                    dataHandler(new DataEvent<T>(stringData, null, Json.OutputOriginalData ? messageEvent.OriginalData : null, messageEvent.ReceivedTimestamp));
#else
                    dataHandler(new DataEvent<T>(stringData, null, null, messageEvent.ReceivedTimestamp));
#endif
                    return;
                }

                var desResult = Json.Deserialize<T>(messageEvent.JsonData, false);
                if (!desResult)
                {
#if DEBUG
                    SocketLog?.Warning($"Socket {connection.Socket.Id} Failed to deserialize data into type {typeof(T)}: {desResult.Error}");
#endif
                    return;
                }
#if DEBUG
                dataHandler(new DataEvent<T>(desResult.Data, null, Json.OutputOriginalData ? messageEvent.OriginalData : null, messageEvent.ReceivedTimestamp));
#else
                dataHandler(new DataEvent<T>(desResult.Data, null, null, messageEvent.ReceivedTimestamp));
#endif
            }

            var subscription = request == null
                ? SocketSubscription.CreateForIdentifier(NextId(), identifier!, userSubscription, InternalHandler)
                : SocketSubscription.CreateForRequest(NextId(), request, userSubscription, InternalHandler);
            connection.AddSubscription(subscription);
            return subscription;
        }

        /// <summary>
        /// Gets a connection for a new subscription or query. Can be an existing if there are open position or a new one.
        /// </summary>
        /// <param name="address">The address the socket is for</param>
        /// <param name="authenticated">Whether the socket should be authenticated</param>
        /// <returns></returns>
        protected virtual SocketConnection GetSocketConnection(string address, bool authenticated)
        {
            var socketResult = sockets.Where(s => s.Value.Socket.Url.TrimEnd('/') == address.TrimEnd('/')
            && s.Value.Connected).OrderBy(s => s.Value.SubscriptionCount).FirstOrDefault();
            var result = socketResult.Equals(default(KeyValuePair<int, SocketConnection>)) ? null : socketResult.Value;
            if (result != null)
            {
                if (result.SubscriptionCount < SocketCombineTarget || (sockets.Count >= MaxSocketConnections && sockets.All(s => s.Value.SubscriptionCount >= SocketCombineTarget)))
                {
                    // Use existing socket if it has less than target connections OR it has the least connections and we can't make new
                    return result;
                }
            }

            // Create new socket
            var socket = CreateSocket(address);
            var socketConnection = new SocketConnection(this, socket);
            socketConnection.UnhandledMessage += HandleUnhandledMessage;
            foreach (var kvp in genericHandlers)
            {
                var handler = SocketSubscription.CreateForIdentifier(NextId(), kvp.Key, false, kvp.Value);
                socketConnection.AddSubscription(handler);
            }

            return socketConnection;
        }

        /// <summary>
        /// Process an unhandled message
        /// </summary>
        /// <param name="token">The token that wasn't processed</param>
        protected virtual void HandleUnhandledMessage(JToken token)
        {
        }

        /// <summary>
        /// Connect a socket
        /// </summary>
        /// <param name="socketConnection">The socket to connect</param>
        /// <returns></returns>
        protected virtual async Task<CallResult<bool>> ConnectSocketAsync(SocketConnection socketConnection)
        {
            if (await socketConnection.Socket.ConnectAsync().ConfigureAwait(false))
            {
                sockets.TryAdd(socketConnection.Socket.Id, socketConnection);
                return new CallResult<bool>(true, null);
            }

            socketConnection.Socket.Dispose();
            return new CallResult<bool>(false, new CantConnectError());
        }

        /// <summary>
        /// Create a socket for an address
        /// </summary>
        /// <param name="address">The address the socket should connect to</param>
        /// <returns></returns>
        protected virtual IWebsocket CreateSocket(string address)
        {
            var socket = SocketFactory.CreateWebsocket(address);
#if DEBUG
            SocketLog?.Debug($"Socket {socket.Id} new socket created for " + address);
#endif
            if (apiProxy != null)
                socket.SetProxy(apiProxy);

            socket.Timeout = SocketNoDataTimeout;
            socket.DataInterpreterBytes = dataInterpreterBytes;
            socket.DataInterpreterString = dataInterpreterString;
            socket.OnError += e =>
            {
#if DEBUG
                if (e is WebSocketException wse)
                    SocketLog?.Warning($"Socket {socket.Id} error: Websocket error code {wse.WebSocketErrorCode}, details: " + e.ToLogString());
                else
                    SocketLog?.Warning($"Socket {socket.Id} error: " + e.ToLogString());
#endif
            };

            return socket;
        }

        /// <summary>
        /// Unsubscribe an update subscription
        /// </summary>
        /// <param name="subscription">The subscription to unsubscribe</param>
        /// <returns></returns>
        public virtual async Task UnsubscribeAsync(UpdateSubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription));
#if DEBUG
            SocketLog?.Info("Closing subscription " + subscription.Id);
#endif
            await subscription.CloseAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Unsubscribe all subscriptions
        /// </summary>
        /// <returns></returns>
        public virtual async Task UnsubscribeAllAsync()
        {
#if DEBUG
            SocketLog?.Debug($"Closing all {sockets.Sum(s => s.Value.SubscriptionCount)} subscriptions");
#endif
            await Task.Run(async () =>
            {
                var tasks = new List<Task>();
                {
                    var socketList = sockets.Values;
                    foreach (var sub in socketList)
                        tasks.Add(sub.CloseAsync());
                }

                await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose the client
        /// </summary>
        public override void Dispose()
        {
#if DEBUG
            SocketLog?.Debug("Disposing socket client, closing all subscriptions");
#endif
            disposing = true;
            periodicEvent?.Set();
            periodicEvent?.Dispose();
            Task.Run(UnsubscribeAllAsync).ConfigureAwait(false).GetAwaiter().GetResult();
            semaphoreSlim?.Dispose();
            base.Dispose();
        }
    }
}