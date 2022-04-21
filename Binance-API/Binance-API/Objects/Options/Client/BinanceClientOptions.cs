using BinanceAPI.Authentication;
using BinanceAPI.Enums;
using BinanceAPI.Options;
using System;
using System.Net.Http;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// Options for the binance client
    /// </summary>
    public class BinanceClientOptions : RestClientOptions
    {
        /// <summary>
        /// The delay before the internal timer starts and begins synchronizing the server time
        /// </summary>
        public int ServerTimeStartTime { get; set; } = 2000;

        /// <summary>
        /// The interval at which to sync the time automatically
        /// </summary>
        public int ServerTimeUpdateTime { get; set; } = 125;

        /// <summary>
        /// Automatic Server Time Sync Update Mode
        /// </summary>
        public ServerTimeSyncType ServerTimeSyncType { get; set; } = ServerTimeSyncType.Manual;

        /// <summary>
        /// The default receive window for requests
        /// </summary>
        public TimeSpan ReceiveWindow { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Constructor with default endpoints
        /// </summary>
        public BinanceClientOptions() : this(BinanceApiAddresses.Default)
        {
        }

        /// <summary>
        /// Constructor with default endpoints
        /// </summary>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public BinanceClientOptions(HttpClient client) : this(BinanceApiAddresses.Default, client)
        {
        }

        /// <summary>
        /// Constructor with custom endpoints
        /// </summary>
        /// <param name="addresses">The base addresses to use</param>
        public BinanceClientOptions(BinanceApiAddresses addresses) : this(addresses.RestClientAddress, null)
        {
        }

        /// <summary>
        /// Constructor with custom endpoints
        /// </summary>
        /// <param name="addresses">The base addresses to use</param>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public BinanceClientOptions(BinanceApiAddresses addresses, HttpClient client) : this(addresses.RestClientAddress, client)
        {
        }

        /// <summary>
        /// Constructor with custom endpoints
        /// </summary>
        /// <param name="spotBaseAddress">Сustom url for the SPOT API</param>
        public BinanceClientOptions(string spotBaseAddress) : this(spotBaseAddress, null)
        {
        }

        /// <summary>
        /// Constructor with custom endpoints
        /// </summary>
        /// <param name="spotBaseAddress">Сustom url for the SPOT API</param>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public BinanceClientOptions(string spotBaseAddress, HttpClient? client) : base(spotBaseAddress)
        {
            HttpClient = client;
        }

        /// <summary>
        /// Return a copy of these options
        /// </summary>
        /// <returns></returns>
        public BinanceClientOptions Copy()
        {
            var copy = Copy<BinanceClientOptions>();
            copy.ServerTimeStartTime = ServerTimeStartTime;
            copy.ServerTimeUpdateTime = ServerTimeUpdateTime;
            copy.ServerTimeSyncType = ServerTimeSyncType;
            copy.ReceiveWindow = ReceiveWindow;
            copy.LogLevel = LogLevel;
            copy.LogPath = LogPath;
            return copy;
        }
    }
}