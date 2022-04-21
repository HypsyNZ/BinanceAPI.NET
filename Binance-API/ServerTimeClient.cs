using BinanceAPI.Enums;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.MarketData;
using BinanceAPI.Requests;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI
{
    /// <summary>
    /// The Client that handles Server Time synchronization
    /// </summary>
    public static class ServerTimeClient
    {
        /// <summary>
        /// True if an instance of the Server Time Client should already exist
        /// </summary>
        public static bool Exists = false;

        /// <summary>
        /// The Current Server Time
        /// </summary>
        public static DateTime ServerTime { get; set; }

        /// <summary>
        /// The used request weight this minute
        /// </summary>
        public static int? UsedWeight { get; set; }

        /// <summary>
        /// The Server Time Offset
        /// </summary>
        public static double CalculatedTimeOffset { get; set; }

        /// <summary>
        /// Dispose Resources for Binance Server Time Updater
        /// </summary>
        public static void Dispose()
        {
            ServerTimeAutomaticSynchronization?.Dispose();
            Client?.Dispose();
            Client = null;
            Exists = false;
        }

        internal static BinanceClient? Client;
        internal static Timer? ServerTimeAutomaticSynchronization;

        internal static void Start(BinanceClientOptions options)
        {
            if (Client != null || Exists)
            {
                return;
            }

            Exists = true;
            Client = new(options);

            switch (options.ServerTimeSyncType)
            {
                case ServerTimeSyncType.Manual:
                    CreateServerTimeUpdater(options.ServerTimeStartTime, options.ServerTimeUpdateTime);
                    break;

                case ServerTimeSyncType.Minute:
                    CreateServerTimeUpdater(2000, 60000);
                    break;

                case ServerTimeSyncType.Hourly:
                    CreateServerTimeUpdater(2000, 3600000);
                    break;

                case ServerTimeSyncType.Aggressive:
                    CreateServerTimeUpdater(2000, 125);
                    break;
            }
        }

        internal static void CreateServerTimeUpdater(int AutoTimestampStartTime, int AutoTimestampTime)
        {
            ServerTimeAutomaticSynchronization?.Dispose();
            ServerTimeAutomaticSynchronization = new(new TimerCallback((o) =>
            {
                try
                {
                    var time = GetServerTimeAsync();
                    if (time.Result.Success)
                    {
                        ServerTime = time.Result.Data;
                        UsedWeight = BinanceHelpers.UsedWeight(time.Result.ResponseHeaders);
                        CalculatedTimeOffset = (time.Result.Data - DateTime.UtcNow).TotalMilliseconds;
                    }
                }
                catch
                {
                    // probably don't have internet connection
                }
            }), null, AutoTimestampStartTime, AutoTimestampTime);
            ServerTimeAutomaticSynchronization.InitializeLifetimeService();
        }

        internal static async Task<WebCallResult<DateTime>> GetServerTimeAsync(CancellationToken ct = default)
        {
            var url = GetUri.New(Client!.BaseAddress, "time", "api", "3");

            var result = await Client.SendRequestInternal<BinanceCheckTime>(url, HttpMethod.Get, ct).ConfigureAwait(false);
            if (!result)
                return new WebCallResult<DateTime>(result.ResponseStatusCode, result.ResponseHeaders, default, result.Error);

            return result.As(result.Data.ServerTime);
        }

        internal static string GetTimestamp()
        {
            return ToUnixTimestamp(DateTime.UtcNow.AddMilliseconds(CalculatedTimeOffset)).ToString(CultureInfo.InvariantCulture);
        }

        internal static long ToUnixTimestamp(DateTime time)
        {
            return (long)(time - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}