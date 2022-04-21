using BinanceAPI.Enums;
using BinanceAPI.Interfaces;
using BinanceAPI.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static BinanceAPI.Logging;

namespace BinanceAPI.Sockets
{
    /// <summary>
    /// Socket connecting
    /// </summary>
    public class SocketConnection
    {
        /// <summary>
        /// Connection lost event
        /// </summary>
        public event Action? ConnectionLost;

        /// <summary>
        /// Connection closed and no reconnect is happening
        /// </summary>
        public event Action? ConnectionClosed;

        /// <summary>
        /// Connecting restored event
        /// </summary>
        public event Action<TimeSpan>? ConnectionRestored;

        /// <summary>
        /// The connection is paused event
        /// </summary>
        public event Action? ActivityPaused;

        /// <summary>
        /// The connection is unpaused event
        /// </summary>
        public event Action? ActivityUnpaused;

        /// <summary>
        /// Connecting closed event
        /// </summary>
        public event Action? Closed;

        /// <summary>
        /// Unhandled message event
        /// </summary>
        public event Action<JToken>? UnhandledMessage;

        /// <summary>
        /// The amount of subscriptions on this connection
        /// </summary>
        public int SubscriptionCount
        {
            get
            {
                lock (subscriptionLock)
                    return subscriptions.Count(h => h.UserSubscription);
            }
        }

        /// <summary>
        /// If connection is made
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// The underlying socket
        /// </summary>
        public IWebsocket Socket { get; set; }

        /// <summary>
        /// If the socket should be reconnected upon closing
        /// </summary>
        public bool ShouldReconnect { get; set; }

        /// <summary>
        /// Current reconnect try
        /// </summary>
        public int ReconnectTry { get; set; }

        /// <summary>
        /// Current resubscribe try
        /// </summary>
        public int ResubscribeTry { get; set; }

        /// <summary>
        /// Time of disconnecting
        /// </summary>
        public DateTime? DisconnectTime { get; set; }

        /// <summary>
        /// If activity is paused
        /// </summary>
        public bool PausedActivity
        {
            get => pausedActivity;
            set
            {
                if (pausedActivity != value)
                {
                    pausedActivity = value;
#if DEBUG
                    SocketLog?.Debug($"Socket {Socket.Id} Paused activity: " + value);
#endif
                    if (pausedActivity) ActivityPaused?.Invoke();
                    else ActivityUnpaused?.Invoke();
                }
            }
        }

        private bool pausedActivity;
        private readonly List<SocketSubscription> subscriptions;
        private readonly object subscriptionLock = new();

        private bool lostTriggered;
        private readonly SocketClient socketClient;

        private readonly List<PendingRequest> pendingRequests;

        /// <summary>
        /// New socket connection
        /// </summary>
        /// <param name="client">The socket client</param>
        /// <param name="socket">The socket</param>
        public SocketConnection(SocketClient client, IWebsocket socket)
        {
            socketClient = client;

            pendingRequests = new List<PendingRequest>();

            subscriptions = new List<SocketSubscription>();
            Socket = socket;

            Socket.Timeout = client.SocketNoDataTimeout;
            Socket.OnMessage += ProcessMessage;
            Socket.OnClose += SocketOnClose;
            Socket.OnOpen += SocketOnOpen;
        }

        /// <summary>
        /// Process a message received by the socket
        /// </summary>
        /// <param name="data"></param>
        private void ProcessMessage(string data)
        {
            var timestamp = DateTime.UtcNow;
#if DEBUG
            SocketLog?.Trace($"Socket {Socket.Id} received data: " + data);
#endif
            if (string.IsNullOrEmpty(data)) return;

#if DEBUG
            var tokenData = data.ToJToken();
            if (tokenData == null)
            {
                data = $"\"{data}\"";
                tokenData = data.ToJToken();
                if (tokenData == null)
                    return;
            }
#else
            var tokenData = data.ToJToken();
            if (tokenData == null)
            {
                data = $"\"{data}\"";
                tokenData = data.ToJToken();
                if (tokenData == null)
                    return;
            }
#endif
            var handledResponse = false;
            PendingRequest[] requests;
            lock (pendingRequests)
                requests = pendingRequests.ToArray();

            // Remove any timed out requests
            foreach (var request in requests.Where(r => r.Completed))
            {
                lock (pendingRequests)
                    pendingRequests.Remove(request);
            }

            // Check if this message is an answer on any pending requests
            foreach (var pendingRequest in requests)
            {
                if (pendingRequest.CheckData(tokenData))
                {
                    lock (pendingRequests)
                        pendingRequests.Remove(pendingRequest);

                    if (!socketClient.ContinueOnQueryResponse)
                        return;

                    handledResponse = true;
                    break;
                }
            }

            // Message was not a request response, check data handlers
#if DEBUG
            var messageEvent = new MessageEvent(this, tokenData, Json.OutputOriginalData ? data : null, timestamp);
#else
            var messageEvent = new MessageEvent(this, tokenData, null, timestamp);
#endif
            if (!HandleData(messageEvent) && !handledResponse)
            {
#if DEBUG
                if (!socketClient.UnhandledMessageExpected)
                    SocketLog?.Warning($"Socket {Socket.Id} Message not handled: " + tokenData);
#endif
                UnhandledMessage?.Invoke(tokenData);
            }
        }

        /// <summary>
        /// Add subscription to this connection
        /// </summary>
        /// <param name="subscription"></param>
        public void AddSubscription(SocketSubscription subscription)
        {
            lock (subscriptionLock)
                subscriptions.Add(subscription);
        }

        /// <summary>
        /// Get a subscription on this connection
        /// </summary>
        /// <param name="id"></param>
        public SocketSubscription GetSubscription(int id)
        {
            lock (subscriptionLock)
                return subscriptions.SingleOrDefault(s => s.Id == id);
        }

        private bool HandleData(MessageEvent messageEvent)
        {
            SocketSubscription? currentSubscription = null;
            try
            {
                var handled = false;

                // Loop the subscriptions to check if any of them signal us that the message is for them
                List<SocketSubscription> subscriptionsCopy;
                lock (subscriptionLock)
                {
                    subscriptionsCopy = subscriptions.ToList();
                }

                foreach (var subscription in subscriptionsCopy)
                {
                    currentSubscription = subscription;
                    if (subscription.Request == null)
                    {
                        handled = true;
                        subscription.MessageHandler(messageEvent);
                    }
                    else
                    {
                        if (socketClient.MessageMatchesHandler(messageEvent.JsonData, subscription.Request))
                        {
                            handled = true;
                            messageEvent.JsonData = socketClient.ProcessTokenData(messageEvent.JsonData);
                            subscription.MessageHandler(messageEvent);
                        }
                    }
                }

                return handled;
            }
            catch (Exception ex)
            {
                SocketLog?.Error($"Socket {Socket.Id} Exception during message processing\r\nException: {ex.ToLogString()}\r\nData: {messageEvent.JsonData}");
                currentSubscription?.InvokeExceptionHandler(ex);
                return false;
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
        public virtual Task SendAndWaitAsync<T>(T obj, TimeSpan timeout, Func<JToken, bool> handler)
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
        public virtual void Send<T>(T obj, NullValueHandling nullValueHandling = NullValueHandling.Ignore)
        {
            if (obj is string str)
                Send(str);
            else
                Send(JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings { NullValueHandling = nullValueHandling }));
        }

        /// <summary>
        /// Send string data over the websocket connection
        /// </summary>
        /// <param name="data">The data to send</param>
        public virtual void Send(string data)
        {
#if DEBUG
            SocketLog?.Debug($"Socket {Socket.Id} sending data: {data}");
#endif
            Socket.Send(data);
        }

        /// <summary>
        /// Handler for a socket opening
        /// </summary>
        protected virtual void SocketOnOpen()
        {
            ReconnectTry = 0;
            PausedActivity = false;
            Connected = true;
        }

        /// <summary>
        /// Handler for a socket closing. Reconnects the socket if needed, or removes it from the active socket list if not
        /// </summary>
        protected virtual void SocketOnClose()
        {
            lock (pendingRequests)
            {
                foreach (var pendingRequest in pendingRequests.ToList())
                {
                    pendingRequest.Fail();
                    pendingRequests.Remove(pendingRequest);
                }
            }

            if (socketClient.AutoReconnect && ShouldReconnect)
            {
                if (Socket.Reconnecting)
                    return; // Already reconnecting

                Socket.Reconnecting = true;
                DisconnectTime = DateTime.UtcNow;
#if DEBUG
                SocketLog?.Info($"Socket {Socket.Id} Connection lost, will try to reconnect after {socketClient.ReconnectInterval}");
#endif
                if (!lostTriggered)
                {
                    lostTriggered = true;
                    ConnectionLost?.Invoke();
                }

                Task.Run(async () =>
                {
                    while (ShouldReconnect)
                    {
                        // Wait a bit before attempting reconnect
                        await Task.Delay(socketClient.ReconnectInterval).ConfigureAwait(false);
                        if (!ShouldReconnect)
                        {
                            // Should reconnect changed to false while waiting to reconnect
                            Socket.Reconnecting = false;
                            return;
                        }

                        Socket.Reset();
                        if (!await Socket.ConnectAsync().ConfigureAwait(false))
                        {
                            ReconnectTry++;
                            ResubscribeTry = 0;
                            if (socketClient.MaxReconnectTries != null
                            && ReconnectTry >= socketClient.MaxReconnectTries)
                            {
#if DEBUG
                                SocketLog?.Debug($"Socket {Socket.Id} failed to reconnect after {ReconnectTry} tries, closing");
#endif
                                ShouldReconnect = false;

                                if (socketClient.sockets.ContainsKey(Socket.Id))
                                    socketClient.sockets.TryRemove(Socket.Id, out _);

                                Closed?.Invoke();
                                _ = Task.Run(() => ConnectionClosed?.Invoke());
                                break;
                            }
#if DEBUG
                            SocketLog?.Debug($"Socket {Socket.Id} failed to reconnect{(socketClient.MaxReconnectTries != null ? $", try {ReconnectTry}/{socketClient.MaxReconnectTries}" : "")}");
#endif
                            continue;
                        }
#if DEBUG
                        SocketLog?.Info($"Socket {Socket.Id} reconnected after {DateTime.UtcNow - DisconnectTime}");
#endif
                        var reconnectResult = await ProcessReconnectAsync().ConfigureAwait(false);
                        if (!reconnectResult)
                        {
                            ResubscribeTry++;

                            if (socketClient.MaxResubscribeTries != null &&
                            ResubscribeTry >= socketClient.MaxResubscribeTries)
                            {
#if DEBUG
                                SocketLog?.Debug($"Socket {Socket.Id} failed to resubscribe after {ResubscribeTry} tries, closing");
#endif
                                ShouldReconnect = false;
                                if (socketClient.sockets.ContainsKey(Socket.Id))
                                    socketClient.sockets.TryRemove(Socket.Id, out _);

                                Closed?.Invoke();
                                _ = Task.Run(() => ConnectionClosed?.Invoke());
                            }
#if DEBUG
                            else

                                SocketLog?.Debug($"Socket {Socket.Id} resubscribing all subscriptions failed on reconnected socket{(socketClient.MaxResubscribeTries != null ? $", try {ResubscribeTry}/{socketClient.MaxResubscribeTries}" : "")}. Disconnecting and reconnecting.");
#endif
                            if (Socket.IsOpen)
                                await Socket.CloseAsync().ConfigureAwait(false);
                            else
                                DisconnectTime = DateTime.UtcNow;
                        }
                        else
                        {
#if DEBUG
                            SocketLog?.Debug($"Socket {Socket.Id} data connection restored.");
#endif
                            ResubscribeTry = 0;
                            if (lostTriggered)
                            {
                                lostTriggered = false;
                                InvokeConnectionRestored(DisconnectTime);
                            }

                            break;
                        }
                    }

                    Socket.Reconnecting = false;
                });
            }
            else
            {
                if (!socketClient.AutoReconnect && ShouldReconnect)
                    _ = Task.Run(() => ConnectionClosed?.Invoke());

#if DEBUG
                SocketLog?.Info($"Socket {Socket.Id} closed");
#endif
                if (socketClient.sockets.ContainsKey(Socket.Id))
                    socketClient.sockets.TryRemove(Socket.Id, out _);

                Closed?.Invoke();
            }
        }

        private async void InvokeConnectionRestored(DateTime? disconnectTime)
        {
            await Task.Run(() => ConnectionRestored?.Invoke(disconnectTime.HasValue ? DateTime.UtcNow - disconnectTime.Value : TimeSpan.FromSeconds(0))).ConfigureAwait(false);
        }

        private async Task<bool> ProcessReconnectAsync()
        {
            // Get a list of all subscriptions on the socket
            List<SocketSubscription> subscriptionList;
            lock (subscriptionLock)
                subscriptionList = subscriptions.Where(h => h.Request != null).ToList();

            // Foreach subscription which is subscribed by a subscription request we will need to resend that request to resubscribe
            for (var i = 0; i < subscriptionList.Count; i += socketClient.MaxConcurrentResubscriptionsPerSocket)
            {
                var success = true;
                var taskList = new List<Task>();
                foreach (var subscription in subscriptionList.Skip(i).Take(socketClient.MaxConcurrentResubscriptionsPerSocket))
                {
                    if (!Socket.IsOpen)
                        continue;

                    var task = socketClient.SubscribeAndWaitAsync(this, subscription.Request!, subscription).ContinueWith(t =>
                    {
                        if (!t.Result)
                            success = false;
                    });
                    taskList.Add(task);
                }

                await Task.WhenAll(taskList).ConfigureAwait(false);
                if (!success || !Socket.IsOpen)
                    return false;
            }
#if DEBUG
            SocketLog?.Debug($"Socket {Socket.Id} all subscription successfully resubscribed on reconnected socket.");
#endif
            return true;
        }

        internal async Task UnsubscribeAsync(SocketSubscription socketSubscription)
        {
            await socketClient.UnsubscribeAsync(this, socketSubscription).ConfigureAwait(false);
        }

        internal async Task<CallResult<bool>> ResubscribeAsync(SocketSubscription socketSubscription)
        {
            if (!Socket.IsOpen)
                return new CallResult<bool>(false, new UnknownError("Socket is not connected"));

            return await socketClient.SubscribeAndWaitAsync(this, socketSubscription.Request!, socketSubscription).ConfigureAwait(false);
        }

        /// <summary>
        /// Close the connection
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            Connected = false;
            ShouldReconnect = false;
            if (socketClient.sockets.ContainsKey(Socket.Id))
                socketClient.sockets.TryRemove(Socket.Id, out _);

            await Socket.CloseAsync().ConfigureAwait(false);
            Socket.Dispose();
        }

        /// <summary>
        /// Close a subscription on this connection. If all subscriptions on this connection are closed the connection gets closed as well
        /// </summary>
        /// <param name="subscription">Subscription to close</param>
        /// <returns></returns>
        public async Task CloseAsync(SocketSubscription subscription)
        {
            if (!Socket.IsOpen)
                return;

            if (subscription.Confirmed)
                await socketClient.UnsubscribeAsync(this, subscription).ConfigureAwait(false);

            var shouldCloseConnection = false;
            lock (subscriptionLock)
                shouldCloseConnection = !subscriptions.Any(r => r.UserSubscription && subscription != r);

            if (shouldCloseConnection)
                await CloseAsync().ConfigureAwait(false);

            lock (subscriptionLock)
                subscriptions.Remove(subscription);
        }
    }

    internal class PendingRequest
    {
        private readonly CancellationTokenSource cts;

        public Func<JToken, bool> Handler { get; }
        public JToken? Result { get; private set; }
        public bool Completed { get; private set; }
        public AsyncResetEvent Event { get; }
        public TimeSpan Timeout { get; }

        public PendingRequest(Func<JToken, bool> handler, TimeSpan timeout)
        {
            Handler = handler;
            Event = new AsyncResetEvent(false, false);
            Timeout = timeout;

            cts = new CancellationTokenSource(timeout);
            cts.Token.Register(Fail, false);
        }

        public bool CheckData(JToken data)
        {
            if (Handler(data))
            {
                Result = data;
                Completed = true;
                Event.Set();
                return true;
            }

            return false;
        }

        public void Fail()
        {
            Completed = true;
            Event.Set();
        }
    }
}