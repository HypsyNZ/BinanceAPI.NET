using BinanceAPI.Authentication;
using BinanceAPI.Interfaces;
using BinanceAPI.Interfaces.SocketSubClient;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Other;
using BinanceAPI.Sockets;
using BinanceAPI.SocketSubClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinanceAPI
{
    /// <summary>
    /// Client providing access to the Binance websocket Api
    /// </summary>
    public class BinanceSocketClient : SocketClient, IBinanceSocketClient
    {
        #region fields

        private static BinanceSocketClientOptions _defaultOptions = new();
        private static BinanceSocketClientOptions DefaultOptions => _defaultOptions.Copy();

        /// <summary>
        /// Spot streams
        /// </summary>
        public IBinanceSocketClientSpot Spot { get; set; }

        #endregion fields

        #region constructor/destructor

        /// <summary>
        /// Create a new instance of BinanceSocketClient with default options
        /// </summary>
        public BinanceSocketClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of BinanceSocketClient using provided options
        /// </summary>
        /// <param name="options">The options to use for this client</param>
        public BinanceSocketClient(BinanceSocketClientOptions options) : base("Binance", options, options.ApiCredentials == null ? null : new AuthenticationProvider(options.ApiCredentials))
        {
            Spot = new BinanceSocketClientSpot(this, options);

            SetDataInterpreter((byte[] data) => { return string.Empty; }, null);
        }

        #endregion constructor/destructor

        #region methods

        /// <summary>
        /// Set the default options to be used when creating new socket clients
        /// </summary>
        /// <param name="options"></param>
        public static void SetDefaultOptions(BinanceSocketClientOptions options)
        {
            _defaultOptions = options;
        }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        public void SetApiCredentials(string apiKey, string apiSecret)
        {
            SetAuthenticationProvider(new AuthenticationProvider(new ApiCredentials(apiKey, apiSecret)));
        }

        internal Task<CallResult<UpdateSubscription>> SubscribeInternal<T>(string url, IEnumerable<string> topics, Action<DataEvent<T>> onData)
        {
            var request = new BinanceSocketRequest
            {
                Method = "SUBSCRIBE",
                Params = topics.ToArray(),
                Id = NextId()
            };

            return SubscribeAsync(url, request, null, false, onData);
        }

        #endregion methods
    }
}