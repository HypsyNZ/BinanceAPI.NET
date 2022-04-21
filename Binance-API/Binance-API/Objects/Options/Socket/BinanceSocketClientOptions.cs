using BinanceAPI.Authentication;
using BinanceAPI.Options;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// Binance socket client options
    /// </summary>
    public class BinanceSocketClientOptions : SocketClientOptions
    {
        /// <summary>
        /// ctor
        /// </summary>
        public BinanceSocketClientOptions() : this(BinanceApiAddresses.Default)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="addresses">The base addresses to use</param>
        public BinanceSocketClientOptions(BinanceApiAddresses addresses) : this(addresses.SocketClientAddress)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="address">Custom address for spot API</param>
        public BinanceSocketClientOptions(string address) : base(address)
        {
            SocketSubscriptionsCombineTarget = 10;
        }

        /// <summary>
        /// Return a copy of these options
        /// </summary>
        /// <returns></returns>
        public BinanceSocketClientOptions Copy()
        {
            var copy = Copy<BinanceSocketClientOptions>();
            return copy;
        }
    }
}