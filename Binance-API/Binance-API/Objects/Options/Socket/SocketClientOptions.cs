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

namespace BinanceAPI.Options
{
    /// <summary>
    /// Base for socket client options
    /// </summary>
    public class SocketClientOptions : ClientOptions
    {
        /// <summary>
        /// The maximum number of times to try to reconnect
        /// </summary>
        public int? MaxReconnectTries { get; set; } = 50;

        /// <summary>
        /// Max number of concurrent resubscription tasks per socket after reconnecting a socket
        /// </summary>
        public int MaxConcurrentResubscriptionsPerSocket { get; set; } = 5;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress">The base address to use</param>
        public SocketClientOptions(string baseAddress) : base(baseAddress)
        {
        }
    }
}
