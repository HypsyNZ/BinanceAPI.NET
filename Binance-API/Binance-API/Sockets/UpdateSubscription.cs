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
using System;
using System.Threading.Tasks;

namespace BinanceAPI.Sockets
{
    /// <summary>
    /// Subscription to a data stream
    /// </summary>
    public class UpdateSubscription
    {
        internal DateTime _lastReconnectAttempt = DateTime.MinValue;

        /// <summary>
        /// The Connection
        /// </summary>
        public readonly SocketConnection Connection;

        private readonly SocketSubscription subscription;

        /// <summary>
        /// Event when the connection is lost. The socket will automatically reconnect when possible.
        /// </summary>
        public event Action ConnectionLost
        {
            add => Connection.ConnectionLost += value;
            remove => Connection.ConnectionLost -= value;
        }

        /// <summary>
        /// Event when the connection is closed and won't be reconnecting
        /// </summary>
        public event Action ConnectionClosed
        {
            add => Connection.ConnectionClosed += value;
            remove => Connection.ConnectionClosed -= value;
        }

        /// <summary>
        /// Event when the connection is restored. Timespan parameter indicates the time the socket has been offline for before reconnecting.
        /// Note that when the executing code is suspended and resumed at a later period (for example laptop going to sleep) the disconnect time will be incorrect as the diconnect
        /// will only be detected after resuming. This will lead to an incorrect disconnected timespan.
        /// </summary>
        public event Action<TimeSpan> ConnectionRestored
        {
            add => Connection.ConnectionRestored += value;
            remove => Connection.ConnectionRestored -= value;
        }

        /// <summary>
        /// Occurs when the status of the socket changes
        /// </summary>
        public event Action<ConnectionStatus>? StatusChanged
        {
            add => Connection.ConnectionStatusChanged += value;
            remove => Connection.ConnectionStatusChanged -= value;
        }

        /// <summary>
        /// The id of the socket
        /// </summary>
        public int SocketId => Connection.BinanceSocket.Id;

        /// <summary>
        /// The id of the subscription
        /// </summary>
        public int Id => subscription.Id;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="connection">The socket connection the subscription is on</param>
        /// <param name="subscription">The subscription</param>
        public UpdateSubscription(SocketConnection connection, SocketSubscription subscription)
        {
            this.Connection = connection;
            this.subscription = subscription;
        }

        /// <summary>
        /// Close the subscription and release the managed resources it is consuming
        /// </summary>
        /// <returns></returns>
        public Task CloseAndDisposeAsync()
        {
            return Connection.CloseAndDisposeSubscriptionAsync(subscription);
        }

        /// <summary>
        /// Close the socket to cause a reconnect
        /// You can only attempt this once every 5 Seconds
        /// </summary>
        /// <returns></returns>
        public async Task ReconnectAsync()
        {
            if (DateTime.UtcNow > (_lastReconnectAttempt + TimeSpan.FromSeconds(5)))
            {
                _lastReconnectAttempt = DateTime.UtcNow;
                await Connection.BinanceSocket.InternalResetAsync().ConfigureAwait(false);
            }
            else
            {
                Logging.SocketLog?.Info("Attempted to reconnect Socket [" + Connection.BinanceSocket.Id + "] when it is likely already reconnecting..");
            }
        }

        /// <summary>
        /// Unsubscribe a subscription
        /// </summary>
        /// <returns></returns>
        internal async Task UnsubscribeAsync()
        {
            await Connection.UnsubscribeAsync(subscription).ConfigureAwait(false);
        }

        /// <summary>
        /// Resubscribe this subscription
        /// </summary>
        /// <returns></returns>
        internal async Task<CallResult<bool>> ResubscribeAsync()
        {
            return await Connection.ResubscribeAsync(subscription).ConfigureAwait(false);
        }
    }
}
