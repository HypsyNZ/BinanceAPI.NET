using BinanceAPI.Interfaces.SocketSubClient;

namespace BinanceAPI.Interfaces
{
    /// <summary>
    /// Interface for subscribing to streams
    /// </summary>
    public interface IBinanceSocketClient : ISocketClient
    {
        /// <summary>
        /// Spot streams
        /// </summary>
        IBinanceSocketClientSpot Spot { get; set; }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);
    }
}