using SimpleLog4.NET;

namespace BinanceAPI
{
    /// <summary>
    /// Log implementation
    /// </summary>
    public static class Logging
    {
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

        internal static void StartClientLog(string clientLogPath, LogLevel clientLogLevel)
        {
            if (clientLogPath != "" && ClientLog == null)
            {
                ClientLog = new(clientLogPath, true, logLevel: clientLogLevel);
            }
        }

        internal static void StartSocketLog(string socketLogPath, LogLevel socketLogLevel)
        {
            if (socketLogPath != "" && SocketLog == null)
            {
                SocketLog = new(socketLogPath, true, logLevel: socketLogLevel);
            }
        }

        internal static void StartOrderBookLog(string orderBookLogPath, LogLevel orderBookLogLogLevel)
        {
            if (orderBookLogPath != "" && OrderBookLog == null)
            {
                OrderBookLog = new(orderBookLogPath, true, logLevel: orderBookLogLogLevel);
            }
        }
    }
}