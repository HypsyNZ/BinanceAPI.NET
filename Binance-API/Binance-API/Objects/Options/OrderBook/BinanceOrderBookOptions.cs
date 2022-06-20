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
using BinanceAPI.Interfaces;
using BinanceAPI.Options;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// Binance symbol order book options
    /// </summary>
    public class BinanceOrderBookOptions : OrderBookOptions
    {
        /// <summary>
        /// The rest client to use for requesting the initial order book
        /// </summary>
        public BinanceClient? RestClient { get; set; }

        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public SocketClient? SocketClient { get; set; }

        /// <summary>
        /// The top amount of results to keep in sync. If for example limit=10 is used, the order book will contain the 10 best bids and 10 best asks. Leaving this null will sync the full order book
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Update interval in milliseconds, either 100 or 1000. Defaults to 1000
        /// </summary>
        public int? UpdateInterval { get; set; }

        /// <summary>
        /// Create new options
        /// </summary>
        /// <param name="limit">The top amount of results to keep in sync. If for example limit=10 is used, the order book will contain the 10 best bids and 10 best asks. Leaving this null will sync the full order book</param>
        /// <param name="updateInterval">Update interval in milliseconds, either 100 or 1000. Defaults to 1000</param>
        /// <param name="socketClient">The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.</param>
        /// <param name="restClient">The rest client to use for requesting the initial order book.</param>
        /// <param name="logPath">Path to the log</param>
        /// <param name="logLevel">Log level for the log</param>
        /// <param name="logToConsole">Ture if messages should also be logged to the console</param>
        public BinanceOrderBookOptions(int? limit = null, int? updateInterval = null, SocketClient? socketClient = null, BinanceClient? restClient = null, string logPath = "", SimpleLog4.NET.LogLevel logLevel = SimpleLog4.NET.LogLevel.Information, bool logToConsole = true) : base("Binance", limit == null, false, "", SimpleLog4.NET.LogLevel.Information, logToConsole)
        {
            Limit = limit;
            UpdateInterval = updateInterval;
            SocketClient = socketClient;
            RestClient = restClient;
            LogLevel = logLevel;
            LogPath = logPath;
        }
    }
}
