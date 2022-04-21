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
        public IBinanceClient? RestClient { get; set; }

        /// <summary>
        /// The client to use for the socket connection. When using the same client for multiple order books the connection can be shared.
        /// </summary>
        public IBinanceSocketClient? SocketClient { get; set; }

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
        public BinanceOrderBookOptions(int? limit = null, int? updateInterval = null, IBinanceSocketClient? socketClient = null, IBinanceClient? restClient = null, string logPath = "", SimpleLog4.NET.LogLevel logLevel = SimpleLog4.NET.LogLevel.Information) : base("Binance", limit == null, false, "", SimpleLog4.NET.LogLevel.Information)
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