namespace BinanceAPI.Authentication
{
    /// <summary>
    /// Api addresses
    /// </summary>
    public class BinanceApiAddresses
    {
        /// <summary>
        /// The address used by the BinanceClient for the Spot API
        /// </summary>
        public string RestClientAddress { get; set; } = "";

        /// <summary>
        /// The address used by the BinanceSocketClient for the Spot API
        /// </summary>
        public string SocketClientAddress { get; set; } = "";

        /// <summary>
        /// The default addresses to connect to the binance.com API
        /// </summary>
        public static BinanceApiAddresses Default = new()
        {
            RestClientAddress = "https://api.binance.com",
            SocketClientAddress = "wss://stream.binance.com:9443/",
        };

        /// <summary>
        /// The addresses to connect to the binance testnet
        /// </summary>
        public static BinanceApiAddresses TestNet = new()
        {
            RestClientAddress = "https://testnet.binance.vision",
            SocketClientAddress = "wss://testnet.binance.vision",
        };

        /// <summary>
        /// The addresses to connect to binance.us.
        /// </summary>
        public static BinanceApiAddresses Us = new()
        {
            RestClientAddress = "https://api.binance.us",
            SocketClientAddress = "wss://stream.binance.us:9443",
        };
    }
}