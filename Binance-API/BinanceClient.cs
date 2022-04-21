using BinanceAPI.Authentication;
using BinanceAPI.Converters;
using BinanceAPI.Enums;
using BinanceAPI.Interfaces;
using BinanceAPI.Interfaces.SubClients;
using BinanceAPI.Interfaces.SubClients.Margin;
using BinanceAPI.Interfaces.SubClients.Spot;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Shared;
using BinanceAPI.Objects.Spot.SpotData;
using BinanceAPI.SubClients;
using BinanceAPI.SubClients.Margin;
using BinanceAPI.SubClients.Spot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI
{
    /// <summary>
    /// Client providing access to the Binance REST Api
    /// </summary>
    public class BinanceClient : RestClient, IBinanceClient
    {
        #region [ Fields ]

        internal readonly TimeSpan DefaultReceiveWindow;

        private static BinanceClientOptions _defaultOptions = new();
        private static BinanceClientOptions DefaultOptions => _defaultOptions.Copy();

        #endregion [ Fields ]

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options"></param>
        public static void SetDefaultOptions(BinanceClientOptions options)
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

        /// <summary>
        /// Create a new instance of BinanceClient using the default options
        /// </summary>
        public BinanceClient() : this(DefaultOptions)
        {
        }

        /// <summary>
        /// Create a new instance of BinanceClient using provided options
        /// </summary>
        /// <param name="options">BinanceClientOptions</param>
        public BinanceClient(BinanceClientOptions options) : base("Binance", options, options.ApiCredentials == null ? null : new AuthenticationProvider(options.ApiCredentials))
        {
            ServerTimeClient.Start(options);

            Logging.StartClientLog(options.LogPath, options.LogLevel);

            DefaultReceiveWindow = options.ReceiveWindow;

            arraySerialization = ArrayParametersSerialization.MultipleValues;
            requestBodyFormat = RequestBodyFormat.FormData;
            requestBodyEmptyContent = string.Empty;

            Spot = new BinanceClientSpot(this);
            Margin = new BinanceClientMargin(this);
            Fiat = new BinanceClientFiat(this);

            General = new BinanceClientGeneral(this);
            Lending = new BinanceClientLending(this);

            WithdrawDeposit = new BinanceClientWithdrawDeposit(this);
        }

        #region [ Sub Clients ]

        /// <summary>
        /// General endpoints
        /// </summary>
        public IBinanceClientGeneral General { get; }

        /// <summary>
        /// (Isolated) Margin endpoints
        /// </summary>
        public IBinanceClientMargin Margin { get; }

        /// <summary>
        /// Spot endpoints
        /// </summary>
        public IBinanceClientSpot Spot { get; }

        /// <summary>
        /// Lending endpoints
        /// </summary>
        public IBinanceClientLending Lending { get; }

        /// <summary>
        /// Withdraw/deposit endpoints
        /// </summary>
        public IBinanceClientWithdrawDeposit WithdrawDeposit { get; }

        /// <summary>
        /// Fiat endpoints
        /// </summary>
        public IBinanceClientFiat Fiat { get; set; }

        #endregion [ Sub Clients ]

        /// <summary>
        /// Event triggered when an order is placed via this client. Only available for Spot orders
        /// </summary>
        public event Action<BinanceOrderBase>? OnOrderPlaced;

        /// <summary>
        /// Event triggered when an order is cancelled via this client. Note that this does not trigger when using CancelAllOrdersAsync. Only available for Spot orders
        /// </summary>
        public event Action<BinanceOrderBase>? OnOrderCanceled;

        #region [Private]

        internal async Task<WebCallResult<BinancePlacedOrder>> PlaceOrderInternal(Uri uri,
            string symbol,
            OrderSide side,
            OrderType type,
            decimal? quantity = null,
            decimal? quoteOrderQuantity = null,
            string? newClientOrderId = null,
            decimal? price = null,
            TimeInForce? timeInForce = null,
            decimal? stopPrice = null,
            decimal? icebergQty = null,
            SideEffectType? sideEffectType = null,
            bool? isIsolated = null,
            OrderResponseType? orderResponseType = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            if (quoteOrderQuantity != null && type != OrderType.Market)
                throw new ArgumentException("quoteOrderQuantity is only valid for market orders");

            if ((quantity == null && quoteOrderQuantity == null) || (quantity != null && quoteOrderQuantity != null))
                throw new ArgumentException("1 of either should be specified, quantity or quoteOrderQuantity");

            var parameters = new Dictionary<string, object>
            {
                { "symbol", symbol },
                { "side", JsonConvert.SerializeObject(side, new OrderSideConverter(false)) },
                { "type", JsonConvert.SerializeObject(type, new OrderTypeConverter(false)) },
                { "timestamp", ServerTimeClient.GetTimestamp() }
            };
            parameters.AddOptionalParameter("quantity", quantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("quoteOrderQty", quoteOrderQuantity?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("newClientOrderId", newClientOrderId);
            parameters.AddOptionalParameter("price", price?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("timeInForce", timeInForce == null ? null : JsonConvert.SerializeObject(timeInForce, new TimeInForceConverter(false)));
            parameters.AddOptionalParameter("stopPrice", stopPrice?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("icebergQty", icebergQty?.ToString(CultureInfo.InvariantCulture));
            parameters.AddOptionalParameter("sideEffectType", sideEffectType == null ? null : JsonConvert.SerializeObject(sideEffectType, new SideEffectTypeConverter(false)));
            parameters.AddOptionalParameter("isIsolated", isIsolated);
            parameters.AddOptionalParameter("newOrderRespType", orderResponseType == null ? null : JsonConvert.SerializeObject(orderResponseType, new OrderResponseTypeConverter(false)));
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequestAsync<BinancePlacedOrder>(uri, HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        /// <inheritdoc />
        protected override Error ParseErrorResponse(JToken error)
        {
            if (!error.HasValues)
                return new ServerError(error.ToString());

            if (error["msg"] == null && error["code"] == null)
                return new ServerError(error.ToString());

            if (error["msg"] != null && error["code"] == null)
                return new ServerError((string)error["msg"]!);

            var err = new ServerError((int)error["code"]!, (string)error["msg"]!);
            if (err.Code == -1021)
            {
                Logging.ClientLog?.Info("Time Is Out Of Sync");
            }
            return err;
        }

        internal Error ParseErrorResponseInternal(JToken error) => ParseErrorResponse(error);

        internal Task<WebCallResult<T>> SendRequestInternal<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object>? parameters = null, bool signed = false, bool checkResult = true, HttpMethodParameterPosition? postPosition = null, ArrayParametersSerialization? arraySerialization = null) where T : class
        {
            return base.SendRequestAsync<T>(uri, method, cancellationToken, parameters, signed, checkResult, postPosition, arraySerialization);
        }

        internal void InvokeOrderPlaced(BinanceOrderBase id)
        {
            OnOrderPlaced?.Invoke(id);
        }

        internal void InvokeOrderCanceled(BinanceOrderBase id)
        {
            OnOrderCanceled?.Invoke(id);
        }

        #endregion [Private]

        /// <summary>
        /// Dispose Resources for this Binance Client
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}