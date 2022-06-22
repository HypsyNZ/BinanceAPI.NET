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

using BinanceAPI.Clients;
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
        /// Unhandled message event
        /// </summary>
        public event Action<JToken>? UnhandledMessage;

        /// <summary>
        /// If connection is made
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// The underlying socket
        /// </summary>
        public WebSocketClient Socket { get; set; }

        /// <summary>
        /// If the socket should be reconnected upon closing
        /// </summary>
        public bool ShouldReconnect { get; set; }

        /// <summary>
        /// Time of disconnecting
        /// </summary>
        public DateTime? DisconnectTime { get; set; }

        private SocketSubscription? Subscription;

        private readonly object subscriptionLock = new();

        private readonly SocketClient socketClient;

        private readonly List<PendingRequest> pendingRequests;

        /// <summary>
        /// New socket connection
        /// </summary>
        /// <param name="client">The socket client</param>
        /// <param name="socket">The socket</param>
        public SocketConnection(SocketClient client, WebSocketClient socket)
        {
            socketClient = client;

            pendingRequests = new List<PendingRequest>();

            Socket = socket;
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

            var tokenData = data.ToJToken();
            if (tokenData == null)
            {
                data = $"\"{data}\"";
                tokenData = data.ToJToken();
                if (tokenData == null)
                    return;
            }

            var handledResponse = false;
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

                    if (!socketClient.ContinueOnQueryResponse)
                    {
                        return;
                    }

                    handledResponse = true;
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
                SocketLog?.Error($"Socket {Socket.Id} Message not handled: " + tokenData.ToString());
                UnhandledMessage?.Invoke(tokenData);
            }
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

        private bool HandleData(MessageEvent messageEvent)
        {
            try
            {
                lock (subscriptionLock)
                {
                    if (socketClient.MessageMatchesHandler(messageEvent.JsonData, Subscription!.Request))
                    {
                        messageEvent.JsonData = socketClient.ProcessTokenData(messageEvent.JsonData);
                        Subscription.MessageHandler(messageEvent);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                SocketLog?.Error($"Socket {Socket.Id} Exception during message processing\r\nException: {ex.ToLogString()}\r\nData: {messageEvent.JsonData}");
            }

            return false;
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
            {
                Send(str);
            }
            else
            {
                Send(JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings { NullValueHandling = nullValueHandling }));
            }
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

            if (ShouldReconnect)
            {
                if (Socket.Reconnecting)
                {
                    return; // Already reconnecting
                }

                Socket.Reconnecting = true;

                _ = Task.Run(() =>
                {
                    DisconnectTime = DateTime.UtcNow;
                    ConnectionLost?.Invoke();
                    SocketLog?.Info($"Socket {Socket.Id} Connection lost, will try to reconnect after 2000ms");
                }).ConfigureAwait(false);

                _ = Task.Run(async () =>
                {
                    var ReconnectTry = 0;
                    var ResubscribeTry = 0;

                    // Connection
                    while (ShouldReconnect)
                    {
                        await Task.Delay(1980).ConfigureAwait(false);

                        // Should reconnect changed
                        if (!ShouldReconnect)
                        {
                            Socket.Reconnecting = false;
                            return;
                        }

                        ReconnectTry++;
                        Socket.NewSocket(true);
                        if (!await Socket.ConnectAsync().ConfigureAwait(false)) // Try Connect
                        {
                            if (ReconnectTry >= socketClient.MaxReconnectTries)
                            {
                                if (socketClient.sockets.ContainsKey(Socket.Id))
                                {
                                    socketClient.sockets.TryRemove(Socket.Id, out _);
                                }

                                //Closed?.Invoke();

                                ShouldReconnect = false;
                                SocketLog?.Debug($"Socket {Socket.Id} failed to reconnect after {ReconnectTry} tries, closing");
                                return; // Failed
                            } // Fail And Return

                            SocketLog?.Debug($"[{DateTime.UtcNow - DisconnectTime}]Socket [{Socket.Id}]  Failed to Reconnect - Attempts: {ReconnectTry}/{socketClient.MaxReconnectTries}");
                            // Try Again
                        }
                        else
                        {
                            SocketLog?.Info($"[{DateTime.UtcNow - DisconnectTime}]Socket [{Socket.Id}] Reconnected - Attempts: {ReconnectTry}/{socketClient.MaxReconnectTries}");
                            break; // Reconnected
                        }
                    }

                    // Subscription
                    while (ShouldReconnect)
                    {
                        var reconnectResult = await ProcessResubscriptionAsync().ConfigureAwait(false);
                        if (!reconnectResult)
                        {
                            ResubscribeTry++;

                            if (ResubscribeTry >= socketClient.MaxReconnectTries)
                            {
                                ShouldReconnect = false;
                                if (socketClient.sockets.ContainsKey(Socket.Id))
                                {
                                    socketClient.sockets.TryRemove(Socket.Id, out _);
                                }

                                //    //Closed?.Invoke();
                                //    // _ = Task.Run(() => ConnectionClosed?.Invoke());

                                SocketLog?.Debug($"Socket {Socket.Id} failed to resubscribe after {ResubscribeTry} tries, closing");
                            }
                            else
                            {
                                SocketLog?.Debug($"Socket {Socket.Id} resubscribing subscription on reconnected socket{(socketClient.MaxReconnectTries != null ? $", try {ResubscribeTry}/{socketClient.MaxReconnectTries}" : "")}. Disconnecting and reconnecting.");
                            }

                            if (Socket.IsOpen)
                            {
                                await CloseAsync().ConfigureAwait(false);
                            }
                            else
                            {
                                DisconnectTime = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            ReconnectTry = 0;
                            ResubscribeTry = 0;
                            SocketLog?.Debug($"Socket {Socket.Id} data connection restored.");

                            _ = Task.Run(() => ConnectionRestored?.Invoke(DisconnectTime.HasValue ? DateTime.UtcNow - DisconnectTime.Value : TimeSpan.FromSeconds(0))).ConfigureAwait(false);

                            break;
                        }
                    }

                    Socket.Reconnecting = false;
                }).ConfigureAwait(false);
            }
            else
            {
                SocketLog?.Info($"Socket {Socket.Id} closed");

                if (socketClient.sockets.ContainsKey(Socket.Id))
                {
                    socketClient.sockets.TryRemove(Socket.Id, out _);
                }

                _ = Task.Run(() =>
                {
                    ConnectionClosed?.Invoke();
                }).ConfigureAwait(false);
            }
        }

        private async Task<bool> ProcessResubscriptionAsync()
        {
            if (!Socket.IsOpen)
            {
                return false;
            }

            var task = await socketClient.SubscribeAndWaitAsync(this, Subscription!.Request!, Subscription).ConfigureAwait(false);

            if (!task.Success || !Socket.IsOpen)
            {
                return false;
            }

            SocketLog?.Debug($"Socket {Socket.Id} subscription successfully resubscribed on reconnected socket.");
            return true;
        }

        internal async Task UnsubscribeAsync(SocketSubscription socketSubscription)
        {
            await socketClient.UnsubscribeAsync(this, socketSubscription).ConfigureAwait(false);
        }

        internal async Task<CallResult<bool>> ResubscribeAsync(SocketSubscription socketSubscription)
        {
            if (!Socket.IsOpen)
            {
                return new CallResult<bool>(false, new UnknownError("Socket is not connected"));
            }

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
            {
                socketClient.sockets.TryRemove(Socket.Id, out _);
            }

            await Socket.CloseAsync().ConfigureAwait(false);
            Socket.Dispose();
        }

        /// <summary>
        /// Closes the subscription on this connection
        /// <para>Closes the connection if the correct subscription is provided</para>
        /// </summary>
        /// <param name="subscription">Subscription to close</param>
        /// <returns></returns>
        public async Task CloseAsync(SocketSubscription subscription)
        {
            if (!Socket.IsOpen)
            {
                return;
            }

            if (subscription.Confirmed)
            {
                var result = await socketClient.UnsubscribeAsync(this, subscription).ConfigureAwait(false);
                if (result)
                {
                    SocketLog?.Debug($"Socket {Socket.Id} subscription successfully unsubscribed, Closing connection");
                    await CloseAsync().ConfigureAwait(false);
                }
            }
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
