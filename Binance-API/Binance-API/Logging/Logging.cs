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

using SimpleLog4.NET;

namespace BinanceAPI
{
    /// <summary>
    /// Log implementation
    /// </summary>
    public static class Logging
    {
        private static object TimeLogLock = new object();
        private static object ClientLogLock = new object();
        private static object SocketLogLock = new object();
        private static object OrderBookLogLock = new object();

        /// <summary>
        /// Binance Time Log
        /// </summary>
        public static Log? TimeLog { get; set; }

        /// <summary>
        /// Binance Client Log
        /// </summary>
        public static Log? ClientLog { get; set; }

        /// <summary>
        /// Socket Client Log
        /// </summary>
        public static Log? SocketLog { get; set; }

        /// <summary>
        /// Order Book Log
        /// </summary>
        public static Log? OrderBookLog { get; set; }

        internal static void StartTimeLog(string timeLogPath, LogLevel timeLogLevel, bool LogToConsole)
        {
            lock (TimeLogLock)
            {
                if (timeLogPath != "" && TimeLog == null)
                {
                    TimeLog = new(timeLogPath, LogToConsole, logLevel: timeLogLevel);
                }
            }
        }

        internal static void StartClientLog(string clientLogPath, LogLevel clientLogLevel, bool LogToConsole)
        {
            lock (ClientLogLock)
            {
                if (clientLogPath != "" && ClientLog == null)
                {
                    ClientLog = new(clientLogPath, LogToConsole, logLevel: clientLogLevel);
                }
            }
        }

        internal static void StartSocketLog(string socketLogPath, LogLevel socketLogLevel, bool LogToConsole)
        {
            lock (SocketLogLock)
            {
                if (socketLogPath != "" && SocketLog == null)
                {
                    SocketLog = new(socketLogPath, LogToConsole, logLevel: socketLogLevel);
                }
            }
        }

        internal static void StartOrderBookLog(string orderBookLogPath, LogLevel orderBookLogLogLevel, bool LogToConsole)
        {
            lock (OrderBookLogLock)
            {
                if (orderBookLogPath != "" && OrderBookLog == null)
                {
                    OrderBookLog = new(orderBookLogPath, LogToConsole, logLevel: orderBookLogLogLevel);
                }
            }
        }
    }
}
