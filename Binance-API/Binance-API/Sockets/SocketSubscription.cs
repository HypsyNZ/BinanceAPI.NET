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

using System;
using System.Diagnostics.CodeAnalysis;

namespace BinanceAPI.Sockets
{
    /// <summary>
    /// Socket subscription
    /// </summary>
    public class SocketSubscription
    {
        /// <summary>
        /// Subscription id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Message handler for this subscription.
        /// </summary>
        public Action<MessageEvent> MessageHandler { get; set; }

        /// <summary>
        /// Request object
        /// </summary>
        [AllowNull]
        public object Request { get; set; }

        /// <summary>
        /// Is user subscription or generic
        /// </summary>
        public bool UserSubscription { get; set; }

        /// <summary>
        /// If the subscription has been confirmed
        /// </summary>
        public bool Confirmed { get; set; }

        private SocketSubscription(int id, [AllowNull] object request, bool userSubscription, Action<MessageEvent> dataHandler)
        {
            Id = id;
            UserSubscription = userSubscription;
            MessageHandler = dataHandler;
            Request = request;
        }

        /// <summary>
        /// Create SocketSubscription for a request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <param name="userSubscription"></param>
        /// <param name="dataHandler"></param>
        /// <returns></returns>
        public static SocketSubscription CreateForRequest(int id, object request, bool userSubscription,
            Action<MessageEvent> dataHandler)
        {
            return new SocketSubscription(id, request, userSubscription, dataHandler);
        }
    }
}
