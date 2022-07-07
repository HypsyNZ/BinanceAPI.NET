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

using BinanceAPI.ClientBase;
using BinanceAPI.ClientHosts;
using BinanceAPI.Enums;
using BinanceAPI.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Connecting restored event
        /// </summary>
        public event Action<TimeSpan>? ConnectionRestored;

        /// <summary>
        /// Occurs when the status of the socket changes
        /// </summary>
        public event Action<ConnectionStatus>? ConnectionStatusChanged;

        /// <summary>
        /// Unhandled message event
        /// </summary>
        public event Action<JToken>? UnhandledMessage;

        /// <summary>
        /// The Current Status of the Socket Connection
        /// </summary>
        public ConnectionStatus SocketConnectionStatus { get; set; }

        /// <summary>
        /// The underlying socket
        /// </summary>
        public BaseSocketClient BinanceSocket { get; set; }

        /// <summary>
        /// If the socket should be reconnected upon closing
        /// </summary>
        public bool ShouldReconnect { get; set; } = true;

        /// <summary>
        /// Time of disconnecting
        /// </summary>
        public DateTime? DisconnectTime { get; set; }

        private SocketSubscription? Subscription;
        private readonly object subscriptionLock = new();
        private readonly SocketClientHost socketClient;
        private readonly List<PendingRequest> pendingRequests;

        /// <summary>
        /// New socket connection
        /// </summary>
        /// <param name="client">The socket client</param>
        /// <param name="socket">The socket</param>
        public SocketConnection(SocketClientHost client, BaseSocketClient socket)
        {
            socketClient = client;

            pendingRequests = new List<PendingRequest>();

            DisconnectTime = DateTime.UtcNow;

            BinanceSocket = socket;
            BinanceSocket.StatusChanged += SocketOnStatusChanged;
            BinanceSocket.OnConnect += SocketOnConnect;
            BinanceSocket.OnMessage += ProcessMessage;
            BinanceSocket.OnClose += SocketOnClose;
            BinanceSocket.OnOpen += SocketOnOpen;
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

        /// <summary>
        /// Send string data over the websocket connection
        /// </summary>
        /// <param name="data">The data to send</param>
        public void Send(string data)
        {
#if DEBUG
            SocketLog?.Debug($"Socket {BinanceSocket.Id} sending data: {data}");
#endif
            BinanceSocket.Send(data);
        }

        /// <summary>
        /// Handler for a socket opening
        /// </summary>
        private void SocketOnOpen()
        {
            SocketOnStatusChanged(ConnectionStatus.Connected);
            SocketLog?.Debug($"Socket {BinanceSocket.Id} connected to {BinanceSocket.Url}");
        }

        private void SocketOnConnect()
        {
            _ = Task.Run(async () =>
            {
                await ReconnectAttempt(true).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        private void SocketOnClose()
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
                if (SocketConnectionStatus == ConnectionStatus.Connecting)
                {
                    return; // Already reconnecting
                }

                DisconnectTime = DateTime.UtcNow;
                SocketOnStatusChanged(ConnectionStatus.Lost);
                SocketLog?.Info($"Socket {BinanceSocket.Id} Connection lost, will try to reconnect after 2000ms");

                _ = Task.Run(async () =>
                {
                    await ReconnectAttempt().ConfigureAwait(false);
                }).ConfigureAwait(false);
            }
            else
            {
                SocketOnStatusChanged(ConnectionStatus.Closed);
                CloseAndDisposeAsync().ConfigureAwait(false);
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
            SocketLog?.Trace($"Socket {BinanceSocket.Id} received data: " + data);
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
            var messageEvent = new MessageEvent(this, tokenData, Json.OutputOriginalData ? data : null, timestamp);
#else
            var messageEvent = new MessageEvent(this, tokenData, null, timestamp);
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

                SocketLog?.Error($"Socket {BinanceSocket.Id} Message not handled: " + tokenData.ToString());
                UnhandledMessage?.Invoke(tokenData);
            }
        }

        private async Task ReconnectAttempt(bool connecting = false)
        {
            if (SocketConnectionStatus == ConnectionStatus.Connecting)
            {
                return; // Already connecting
            }

            var ReconnectTry = 0;
            var ResubscribeTry = 0;

            // Connection
            while (ShouldReconnect)
            {
                await Task.Delay(1984).ConfigureAwait(false);

                if (!ShouldReconnect)
                {
                    DisconnectTime = DateTime.UtcNow;
                    SocketOnStatusChanged(ConnectionStatus.Disconnected);
                    return;
                }

                ReconnectTry++;
                BinanceSocket.NewSocket(true);
                if (!await BinanceSocket.ConnectAsync().ConfigureAwait(false))
                {
                    if (ReconnectTry >= socketClient.MaxReconnectTries)
                    {
                        if (socketClient.AllSockets.ContainsKey(BinanceSocket.Id))
                        {
                            socketClient.AllSockets.TryRemove(BinanceSocket.Id, out _);
                        }

                        ShouldReconnect = false;
                        SocketLog?.Debug($"Socket {BinanceSocket.Id} failed to {(connecting ? "connect" : "reconnect")} after {ReconnectTry} tries, closing");
                        return;
                    }

                    SocketLog?.Debug($"[{DateTime.UtcNow - DisconnectTime}] Socket [{BinanceSocket.Id}]  Failed to {(connecting ? "Connect" : "Reconnect")} - Attempts: {ReconnectTry}/{socketClient.MaxReconnectTries}");
                }
                else
                {
                    SocketLog?.Info($"[{DateTime.UtcNow - DisconnectTime}] Socket [{BinanceSocket.Id}] {(connecting ? "Connected" : "Reconnected")} - Attempts: {ReconnectTry}/{socketClient.MaxReconnectTries}");
                    break;
                }
            }

            // Subscription
            while (ShouldReconnect)
            {
                var reconnectResult = await ProcessResubscriptionAsync(connecting).ConfigureAwait(false);
                if (!reconnectResult)
                {
                    ResubscribeTry++;

                    if (ResubscribeTry >= socketClient.MaxReconnectTries)
                    {
                        ShouldReconnect = false;
                        if (socketClient.AllSockets.ContainsKey(BinanceSocket.Id))
                        {
                            socketClient.AllSockets.TryRemove(BinanceSocket.Id, out _);
                        }
                        SocketLog?.Debug($"Socket {BinanceSocket.Id} failed to {(connecting ? "subscribe" : "resubscribe")} after {ResubscribeTry} tries, closing");

                        SocketOnStatusChanged(ConnectionStatus.Closed);
                    }
                    else
                    {
                        SocketLog?.Debug($"Socket {BinanceSocket.Id}  {(connecting ? "subscribing" : "resubscribing")} subscription on  {(connecting ? "connected" : "reconnected")} socket{(socketClient.MaxReconnectTries != null ? $", try {ResubscribeTry}/{socketClient.MaxReconnectTries}" : "")}. Disconnecting and reconnecting.");
                    }

                    if (BinanceSocket.IsOpen)
                    {
                        await CloseAndDisposeAsync().ConfigureAwait(false);
                    }
                }
                else
                {
                    ReconnectTry = 0;
                    ResubscribeTry = 0;
                    SocketLog?.Debug($"Socket {BinanceSocket.Id} data connection {(connecting ? "made" : "restored")}.");
                    _ = Task.Run(() => ConnectionRestored?.Invoke(DisconnectTime.HasValue ? DateTime.UtcNow - DisconnectTime.Value : TimeSpan.FromSeconds(0))).ConfigureAwait(false);

                    break;
                }
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
                        messageEvent.JsonData = (JToken)messageEvent.JsonData;
                        Subscription.MessageHandler(messageEvent);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                SocketLog?.Error($"Socket {BinanceSocket.Id} Exception during message processing Data: {messageEvent.JsonData}", ex);
            }

            return false;
        }

        private async Task<bool> ProcessResubscriptionAsync(bool connecting = false)
        {
            if (!BinanceSocket.IsOpen)
            {
                return false;
            }

            var task = await socketClient.SubscribeAndWaitAsync(this, Subscription!.Request!, Subscription).ConfigureAwait(false);

            if (!task.Success || !BinanceSocket.IsOpen)
            {
                return false;
            }

            SocketLog?.Debug($"Socket {BinanceSocket.Id} subscription successfully {(connecting ? "subscribed" : "resubscribed")} on {(connecting ? "connected" : "reconnected")} socket.");
            return true;
        }

        internal async Task UnsubscribeAsync(SocketSubscription socketSubscription)
        {
            await socketClient.UnsubscribeAsync(this, socketSubscription).ConfigureAwait(false);
        }

        internal async Task<CallResult<bool>> ResubscribeAsync(SocketSubscription socketSubscription)
        {
            if (!BinanceSocket.IsOpen)
            {
                return new CallResult<bool>(false, new UnknownError("Socket is not connected"));
            }

            return await socketClient.SubscribeAndWaitAsync(this, socketSubscription.Request!, socketSubscription).ConfigureAwait(false);
        }

        /// <summary>
        /// Close the connection and releases the managed resources it is consuming.
        /// </summary>
        /// <returns></returns>
        public async Task CloseAndDisposeAsync()
        {
            SocketLog?.Info($"Socket {BinanceSocket.Id} closed");
            ShouldReconnect = false;
            await BinanceSocket.InternalResetAsync().ConfigureAwait(false);

            if (socketClient.AllSockets.ContainsKey(BinanceSocket.Id))
            {
                socketClient.AllSockets.TryRemove(BinanceSocket.Id, out _);
            }

            DisconnectTime = DateTime.UtcNow;
            BinanceSocket.Dispose();
        }

        /// <summary>
        /// Closes the subscription on this connection
        /// <para>Closes the connection if the correct subscription is provided</para>
        /// </summary>
        /// <param name="subscription">Subscription to close</param>
        /// <returns></returns>
        public async Task CloseAndDisposeSubscriptionAsync(SocketSubscription subscription)
        {
            if (!BinanceSocket.IsOpen)
            {
                return;
            }

            if (subscription.Confirmed)
            {
                var result = await socketClient.UnsubscribeAsync(this, subscription).ConfigureAwait(false);
                if (result)
                {
                    SocketLog?.Debug($"Socket {BinanceSocket.Id} subscription successfully unsubscribed, Closing connection..");
                }
                else
                {
                    SocketLog?.Debug($"Socket {BinanceSocket.Id} subscription seems to already be unsubscribed, Closing connection..");
                }

                await CloseAndDisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
