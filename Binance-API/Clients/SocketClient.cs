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

using BinanceAPI.Objects;
using BinanceAPI.Objects.Other;
using BinanceAPI.Options;
using BinanceAPI.Sockets;
using BinanceAPI.SocketSubClients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using static BinanceAPI.Logging;

namespace BinanceAPI.Clients
{
    /// <summary>
    /// Base for socket client implementations
    /// </summary>
    public class SocketClient : BaseClient
    {
        /// <summary>
        /// The Default Options or the Options that you Set
        /// <para>new BinanceSocketClientOptions() creates the standard defaults regardless of what you set this to</para>
        /// </summary>
        public static BinanceSocketClientOptions DefaultOptions = new();

        /// <summary>
        /// Spot streams
        /// </summary>
        public BinanceSocketClientSpot Spot { get; set; }

        #region constructor/destructor

        /// <summary>
        /// Create a new instance of BinanceSocketClient with default options
        /// </summary>
        public SocketClient() : this(DefaultOptions)
        {
        }

        #endregion constructor/destructor

        #region methods

        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options"></param>
        public static void SetDefaultOptions(BinanceSocketClientOptions options)
        {
            DefaultOptions = options;
        }

        internal Task<CallResult<UpdateSubscription>> SubscribeInternal<T>(string url, IEnumerable<string> topics, Action<DataEvent<T>> onData)
        {
            var request = new BinanceSocketRequest
            {
                Method = "SUBSCRIBE",
                Params = topics.ToArray(),
                Id = NextId()
            };

            return SubscribeAsync(url, request, false, onData);
        }

        #endregion methods

        #region fields

        /// <summary>
        /// List of socket connections currently connecting/connected
        /// </summary>
        protected internal ConcurrentDictionary<int, SocketConnection> sockets = new();

        /// <summary>
        /// The max amount of concurrent socket connections
        /// </summary>
        public int MaxSocketConnections { get; protected set; } = 9999;

        /// <inheritdoc cref="SocketClientOptions.MaxReconnectTries"/>
        public int? MaxReconnectTries { get; protected set; }

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
        /// The current kilobytes per second of data being received by all connection from this client, averaged over the last 3 seconds
        /// </summary>
        public double IncomingKbps
        {
            get
            {
                if (!sockets.Any())
                    return 0;

                return sockets.Sum(s => s.Value.BinanceSocket.IncomingKbps);
            }
        }

        #endregion fields

        /// <summary>
        /// Create a new instance of BinanceSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public SocketClient(BinanceSocketClientOptions options) : base(options)
        {
            Spot = new BinanceSocketClientSpot(this, options);

            SetDataInterpreter((data) => { return string.Empty; }, null);

            if (options == null)
                throw new ArgumentNullException(nameof(options));

            MaxReconnectTries = options.MaxReconnectTries;
            MaxConcurrentResubscriptionsPerSocket = options.MaxConcurrentResubscriptionsPerSocket;

            StartSocketLog(options.LogPath, options.LogLevel, options.LogToConsole);
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
        /// <param name="authenticated">If the subscription is to an authenticated endpoint</param>
        /// <param name="dataHandler">The handler of update data</param>
        /// <returns></returns>
        protected async Task<CallResult<UpdateSubscription>> SubscribeAsync<T>(string url, object request, bool authenticated, Action<DataEvent<T>> dataHandler)
        {
            SocketConnection socketConnection;
            SocketSubscription subscription;

            // Get a new or existing socket connection
            socketConnection = new SocketConnection(this, CreateSocket(url));
            socketConnection.UnhandledMessage += HandleUnhandledMessage;

            // Add a subscription on the socket connection
            subscription = AddSubscription(request, true, socketConnection, dataHandler);

            var needsConnecting = !socketConnection.Connected;

            var connectResult = await ConnectIfNeededAsync(socketConnection, authenticated).ConfigureAwait(false);
            if (!connectResult)
                return new CallResult<UpdateSubscription>(null, connectResult.Error);
            if (needsConnecting)
                SocketLog?.Debug($"Socket {socketConnection.BinanceSocket.Id} connected to {url} {(request == null ? "" : "with request " + JsonConvert.SerializeObject(request))}");
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
        protected internal async Task<CallResult<bool>> SubscribeAndWaitAsync(SocketConnection socketConnection, object request, SocketSubscription subscription)
        {
            CallResult<object>? callResult = null;
            await socketConnection.SendAndWaitAsync(request, TimeSpan.FromSeconds(3), data => HandleSubscriptionResponse(request, data, out callResult)).ConfigureAwait(false);

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
        protected async Task<CallResult<bool>> ConnectIfNeededAsync(SocketConnection socket, bool authenticated)
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
        public bool MessageMatchesHandler(JToken message, object? request)
        {
            if (request == null)
                return false;

            if (message.Type != JTokenType.Object)
                return false;

            var bRequest = (BinanceSocketRequest)request;
            var stream = message["stream"];
            if (stream == null)
                return false;

            return bRequest.Params.Contains(stream.ToString());
        }

        /// <summary>
        /// Needs to unsubscribe a subscription, typically by sending an unsubscribe request.
        /// </summary>
        /// <param name="connection">The connection on which to unsubscribe</param>
        /// <param name="subscription">The subscription to unsubscribe</param>
        /// <returns></returns>
        public async Task<bool> UnsubscribeAsync(SocketConnection connection, SocketSubscription subscription)
        {
            var result = false;
            var topics = ((BinanceSocketRequest)subscription.Request!).Params;

            var unsub = new BinanceSocketRequest
            {
                Method = "UNSUBSCRIBE",
                Params = topics,
                Id = NextId()
            };

            if (!connection.BinanceSocket.IsOpen)
                return true;

            await connection.SendAndWaitAsync(unsub, TimeSpan.FromSeconds(3),
                data =>
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
        /// Add a subscription to a connection
        /// </summary>
        /// <typeparam name="T">The type of data the subscription expects</typeparam>
        /// <param name="request">The request of the subscription</param>
        /// <param name="userSubscription">Whether or not this is a user subscription (counts towards the max amount of handlers on a socket)</param>
        /// <param name="connection">The socket connection the handler is on</param>
        /// <param name="dataHandler">The handler of the data received</param>
        /// <returns></returns>
        protected SocketSubscription AddSubscription<T>(object request, bool userSubscription, SocketConnection connection, Action<DataEvent<T>> dataHandler)
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
                    SocketLog?.Warning($"Socket {connection.BinanceSocket.Id} Failed to deserialize data into type {typeof(T)}: {desResult.Error}");
                    return;
                }
#if DEBUG
                dataHandler(new DataEvent<T>(desResult.Data, null, Json.OutputOriginalData ? messageEvent.OriginalData : null, messageEvent.ReceivedTimestamp));
#else
                dataHandler(new DataEvent<T>(desResult.Data, null, null, messageEvent.ReceivedTimestamp));
#endif
            }

            var subscription = SocketSubscription.CreateForRequest(NextId(), request, userSubscription, InternalHandler);
            connection.AddSubscription(subscription);
            return subscription;
        }

        /// <summary>
        /// Process an unhandled message
        /// </summary>
        /// <param name="token">The token that wasn't processed</param>
        protected void HandleUnhandledMessage(JToken token)
        {
        }

        /// <summary>
        /// Connect a socket
        /// </summary>
        /// <param name="socketConnection">The socket to connect</param>
        /// <returns></returns>
        protected async Task<CallResult<bool>> ConnectSocketAsync(SocketConnection socketConnection)
        {
            if (await socketConnection.BinanceSocket.ConnectAsync().ConfigureAwait(false))
            {
                sockets.TryAdd(socketConnection.BinanceSocket.Id, socketConnection);
                return new CallResult<bool>(true, null);
            }

            socketConnection.BinanceSocket.Dispose();
            return new CallResult<bool>(false, new CantConnectError());
        }

        /// <summary>
        /// Create a socket for an address
        /// </summary>
        /// <param name="address">The address the socket should connect to</param>
        /// <returns></returns>
        protected BaseSocketClient CreateSocket(string address)
        {
            BaseSocketClient socket = new BaseSocketClient(address);
#if DEBUG
            SocketLog?.Debug($"Socket {socket.Id} new socket created for " + address);
#endif
            if (apiProxy != null)
                socket.SetProxy(apiProxy);

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
        public async Task UnsubscribeAsync(UpdateSubscription subscription)
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
        public async Task UnsubscribeAllAsync()
        {
#if DEBUG
            SocketLog?.Debug($"Closing all {sockets.Count} subscriptions");
#endif
            await Task.Run(async () =>
            {
                var tasks = new List<Task>();
                {
                    var socketList = sockets.Values;
                    foreach (var sub in socketList)
                    {
                        tasks.Add(sub.CloseAsync());
                    }
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
            base.Dispose();
        }
    }
}
