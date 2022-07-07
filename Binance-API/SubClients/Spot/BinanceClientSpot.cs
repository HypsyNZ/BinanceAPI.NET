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

using BinanceAPI.ClientHosts;
using BinanceAPI.Interfaces.SubClients;
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
    public class BinanceClientSpot
    {
        private const string tradingStatusEndpoint = "account/apiTradingStatus";

        /// <summary>
        /// Spot system endpoints
        /// </summary>
        public BinanceClientSpotSystem System { get; }

        /// <summary>
        /// Spot market endpoints
        /// </summary>
        public BinanceClientSpotMarket Market { get; }

        /// <summary>
        /// Spot order endpoints
        /// </summary>
        public BinanceClientSpotOrder Order { get; }

        /// <summary>
        /// Spot user stream endpoints
        /// </summary>
        public IBinanceClientUserStream UserStream { get; }

        private readonly BinanceClientHost _baseClient;

        internal BinanceClientSpot(BinanceClientHost baseClient)
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
            var parameters = new Dictionary<string, object>();

            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.SendRequestInternal<BinanceResult<BinanceTradingStatus>>(UriClient.GetBaseAddress() + GetUriString.Combine(tradingStatusEndpoint, "sapi", "1"), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);
            if (!result)
                return new WebCallResult<BinanceTradingStatus>(result.ResponseStatusCode, result.ResponseHeaders, null, result.Error);

            return !string.IsNullOrEmpty(result.Data.Message) ? new WebCallResult<BinanceTradingStatus>(result.ResponseStatusCode, result.ResponseHeaders, null, new ServerError(result.Data.Message!)) : result.As(result.Data.Data);
        }

        #endregion Trading status
    }
}
