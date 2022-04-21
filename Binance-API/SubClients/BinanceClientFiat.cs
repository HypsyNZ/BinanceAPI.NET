using BinanceAPI.Converters;
using BinanceAPI.Enums;
using BinanceAPI.Interfaces.SubClients;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Fiat;
using BinanceAPI.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.SubClients
{
    /// <summary>
    /// Liquid swap endpoints
    /// </summary>
    public class BinanceClientFiat : IBinanceClientFiat
    {
        private const string api = "sapi";
        private const string apiVersion = "1";

        private const string fiatDepositWithdrawHistoryEndpoint = "fiat/orders";
        private const string fiatPaymentHistoryEndpoint = "fiat/payments";

        private readonly BinanceClient _baseClient;

        internal BinanceClientFiat(BinanceClient baseClient)
        {
            _baseClient = baseClient;
        }

        #region Get Fiat Payments History

        /// <summary>
        /// Get Fiat payment history
        /// </summary>
        /// <param name="side">Filter by side</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Return a specific page</param>
        /// <param name="limit">The page size</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<BinanceFiatPayment>>> GetFiatPaymentHistoryAsync(OrderSide side, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "transactionType", side == OrderSide.Buy ? "0": "1" },
                {"timestamp", ServerTimeClient.GetTimestamp()}
            };
            parameters.AddOptionalParameter("beginTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("page", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.SendRequestInternal<BinanceResult<IEnumerable<BinanceFiatPayment>>>(GetUri.New(_baseClient.BaseAddress, fiatPaymentHistoryEndpoint, api, apiVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);

            return result.As(result.Data?.Data!);
        }

        #endregion Get Fiat Payments History

        #region Get Fiat Deposit Withdraw History

        /// <summary>
        /// Get Fiat deposit/withdrawal history
        /// </summary>
        /// <param name="side">Filter by side</param>
        /// <param name="startTime">Filter by start time</param>
        /// <param name="endTime">Filter by end time</param>
        /// <param name="page">Return a specific page</param>
        /// <param name="limit">The page size</param>
        /// <param name="receiveWindow">The receive window for which this request is active. When the request takes longer than this to complete the server will reject the request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<IEnumerable<BinanceFiatWithdrawDeposit>>> GetFiatDepositWithdrawHistoryAsync(TransactionType side, DateTime? startTime = null, DateTime? endTime = null, int? page = null, int? limit = null, int? receiveWindow = null, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>
            {
                { "transactionType", side == TransactionType.Deposit ? "0": "1" },
                {"timestamp", ServerTimeClient.GetTimestamp()}
            };
            parameters.AddOptionalParameter("beginTime", startTime.HasValue ? JsonConvert.SerializeObject(startTime.Value, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("endTime", endTime.HasValue ? JsonConvert.SerializeObject(endTime.Value, new TimestampConverter()) : null);
            parameters.AddOptionalParameter("page", page?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("limit", limit?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? _baseClient.DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            var result = await _baseClient.SendRequestInternal<BinanceResult<IEnumerable<BinanceFiatWithdrawDeposit>>>(GetUri.New(_baseClient.BaseAddress, fiatDepositWithdrawHistoryEndpoint, api, apiVersion), HttpMethod.Get, ct, parameters, true).ConfigureAwait(false);

            return result.As(result.Data?.Data!);
        }

        #endregion Get Fiat Deposit Withdraw History
    }
}