using BinanceAPI.Interfaces.SubClients;
using BinanceAPI.Interfaces.SubClients.Spot;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.WalletData;
using BinanceAPI.Requests;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.SubClients.Spot
{
    /// <summary>
    /// Spot endpoints
    /// </summary>
    public class BinanceClientSpot : IBinanceClientSpot
    {
        private const string tradingStatusEndpoint = "account/apiTradingStatus";

        /// <summary>
        /// Spot system endpoints
        /// </summary>
        public IBinanceClientSpotSystem System { get; }

        /// <summary>
        /// Spot market endpoints
        /// </summary>
        public IBinanceClientSpotMarket Market { get; }

        /// <summary>
        /// Spot order endpoints
        /// </summary>
        public IBinanceClientSpotOrder Order { get; }

        /// <summary>
        /// Spot user stream endpoints
        /// </summary>
        public IBinanceClientUserStream UserStream { get; }

        private readonly BinanceClient _baseClient;

        internal BinanceClientSpot(BinanceClient baseClient)
        {
            _baseClient = baseClient;

            System = new BinanceClientSpotSystem(baseClient);
            Market = new BinanceClientSpotMarket(baseClient);
            Order = new BinanceClientSpotOrder(baseClient);
            UserStream = new BinanceClientSpotUserStream(baseClient);
        }

        #region Trading status

        /// <summary>
        /// Gets the trading status for the current account
        /// </summary>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>The trading status of the account</returns>
        public async Task<WebCallResult<BinanceTradingStatus>> GetTradingStatusAsync(int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "timestamp", ServerTimeClient.GetTimestamp() },
            };

            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.SendRequestInternal<BinanceResult<BinanceTradingStatus>>(GetUri.New(_baseClient.BaseAddress, tradingStatusEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return new WebCallResult<BinanceTradingStatus>(result.ResponseStatusCode, result.ResponseHeaders, null, result.Error);

            return !string.IsNullOrEmpty(result.Data.Message) ? new WebCallResult<BinanceTradingStatus>(result.ResponseStatusCode, result.ResponseHeaders, null, new ServerError(result.Data.Message!)) : result.As(result.Data.Data);
        }

        #endregion Trading status
    }
}