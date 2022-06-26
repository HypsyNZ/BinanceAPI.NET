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

using BinanceAPI.Clients;
using BinanceAPI.Converters;
using BinanceAPI.Enums;
using BinanceAPI.Interfaces;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Shared;
using BinanceAPI.Objects.Spot.SpotData;
using BinanceAPI.Options;
using BinanceAPI.Requests;
using BinanceAPI.SubClients;
using BinanceAPI.SubClients.Margin;
using BinanceAPI.SubClients.Spot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleLog4.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using static BinanceAPI.Logging;

namespace BinanceAPI
{
    /// <summary>
    /// Base Rest Client
    /// </summary>
    public class BinanceClientHost : BaseClient
    {
        internal readonly TimeSpan DefaultReceiveWindow;

        /// <summary>
        /// <para>Spot</para>
        /// Event triggered when an order is placed via any client.
        /// </summary>
        public static EventHandler<BinanceOrderBase>? OnOrderPlaced;

        /// <summary>
        /// <para>Spot</para>
        /// <para>Note that this does not trigger when using CancelAllOrdersAsync.</para>
        /// Event triggered when an order is cancelled via any client.
        /// </summary>
        public static EventHandler<BinanceOrderBase>? OnOrderCanceled;

        /// <summary>
        /// The Default Options or the Options that you Set
        /// <para>new BinanceClientOptions() creates the standard defaults regardless of what you set this to</para>
        /// </summary>
        public static BinanceClientHostOptions DefaultOptions { get; set; } = new();

        /// <summary>
        /// Set the default options to be used when creating new clients
        /// </summary>
        /// <param name="options"></param>
        public static void SetDefaultOptions(BinanceClientHostOptions options)
        {
            DefaultOptions = options;
        }

        /// <summary>
        /// Create a new instance of BinanceClient using the default options
        /// </summary>
        public BinanceClientHost(CancellationToken waitToken = default) : this(DefaultOptions, waitToken)
        {
        }

        /// <summary>
        /// Create a new instance of BinanceClient using provided options
        /// </summary>
        /// <param name="options">BinanceClientOptions</param>
        /// <param name="waitToken">Wait token for Server Time Client</param>
        public BinanceClientHost(BinanceClientHostOptions options, CancellationToken waitToken) : base(options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            ServerTimeClient.Start(options, waitToken).ConfigureAwait(false);
            Logging.StartClientLog(options.LogPath, options.LogLevel, options.LogToConsole);
            Logging.ClientLog?.Info("Started Binance Client");

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

            RequestTimeout = options.RequestTimeout;
            RequestFactory.Configure(options.RequestTimeout, options.Proxy, options.HttpClient);
        }

        #region [ Sub Clients ]

        /// <summary>
        /// General endpoints
        /// </summary>
        public BinanceClientGeneral General { get; }

        /// <summary>
        /// (Isolated) Margin endpoints
        /// </summary>
        public BinanceClientMargin Margin { get; }

        /// <summary>
        /// Spot endpoints
        /// </summary>
        public BinanceClientSpot Spot { get; }

        /// <summary>
        /// Lending endpoints
        /// </summary>
        public BinanceClientLending Lending { get; }

        /// <summary>
        /// Withdraw/deposit endpoints
        /// </summary>
        public BinanceClientWithdrawDeposit WithdrawDeposit { get; }

        /// <summary>
        /// Fiat endpoints
        /// </summary>
        public BinanceClientFiat Fiat { get; set; }

        #endregion [ Sub Clients ]

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
            int? trailingDelta = null,
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
                { "type", JsonConvert.SerializeObject(type, new OrderTypeConverter(false)) }
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
            parameters.AddOptionalParameter("trailingDelta", trailingDelta);
            parameters.AddOptionalParameter("recvWindow", receiveWindow?.ToString(CultureInfo.InvariantCulture) ?? DefaultReceiveWindow.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));

            return await SendRequestAsync<BinancePlacedOrder>(uri, HttpMethod.Post, ct, parameters, true).ConfigureAwait(false);
        }

        internal Error ParseErrorResponseInternal(JToken error) => ParseErrorResponse(error);

        internal Task<WebCallResult<T>> SendRequestInternal<T>(Uri uri, HttpMethod method, CancellationToken cancellationToken,
            Dictionary<string, object> parameters, bool signed = false, bool checkResult = true, HttpMethodParameterPosition? postPosition = null, ArrayParametersSerialization? arraySerialization = null) where T : class
        {
            //  parameters?.AddParameter("timestamp", ServerTimeClient.GetTimestamp());
            return SendRequestAsync<T>(uri, method, cancellationToken, parameters, signed, checkResult, postPosition, arraySerialization);
        }

        #endregion [Private]

        /// <summary>
        /// Dispose Resources for this Binance Client
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }

        /// <summary>
        /// The factory for creating requests. Used for unit testing
        /// </summary>
        public RequestFactory RequestFactory { get; set; } = new RequestFactory();

        /// <summary>
        /// Where to put the parameters for requests with different Http methods
        /// </summary>
        protected Dictionary<HttpMethod, HttpMethodParameterPosition> ParameterPositions { get; set; } = new Dictionary<HttpMethod, HttpMethodParameterPosition>
        {
            { HttpMethod.Get, HttpMethodParameterPosition.InUri },
            { HttpMethod.Post, HttpMethodParameterPosition.InBody },
            { HttpMethod.Delete, HttpMethodParameterPosition.InBody },
            { HttpMethod.Put, HttpMethodParameterPosition.InBody }
        };

        /// <summary>
        /// Request body content type
        /// </summary>
        protected RequestBodyFormat requestBodyFormat = RequestBodyFormat.Json;

        /// <summary>
        /// Whether or not we need to manually parse an error instead of relying on the http status code
        /// </summary>
        protected bool manualParseError = false;

        /// <summary>
        /// How to serialize array parameters when making requests
        /// </summary>
        protected ArrayParametersSerialization arraySerialization = ArrayParametersSerialization.Array;

        /// <summary>
        /// What request body should be set when no data is send (only used in combination with postParametersPosition.InBody)
        /// </summary>
        protected string requestBodyEmptyContent = "{}";

        /// <summary>
        /// Timeout for requests. This setting is ignored when injecting a HttpClient in the options, requests timeouts should be set on the client then.
        /// </summary>
        public TimeSpan RequestTimeout { get; }

        /// <summary>
        /// Total requests made by this client
        /// </summary>
        public int TotalRequestsMade { get; private set; }

        /// <summary>
        /// Request headers to be sent with each request
        /// </summary>
        protected Dictionary<string, string>? StandardRequestHeaders { get; set; }

        /// <summary>
        /// Ping to see if the server is reachable
        /// </summary>
        /// <returns>The roundtrip time of the ping request</returns>
        public CallResult<long> Ping(CancellationToken ct = default) => PingAsync(ct).Result;

        /// <summary>
        /// Ping to see if the server is reachable
        /// </summary>
        /// <returns>The roundtrip time of the ping request</returns>
        public async Task<CallResult<long>> PingAsync(CancellationToken ct = default)
        {
            var ping = new Ping();
            var uri = new Uri(BaseAddress);
            PingReply reply;

            var ctRegistration = ct.Register(() => ping.SendAsyncCancel());
            try
            {
                reply = await ping.SendPingAsync(uri.Host).ConfigureAwait(false);
            }
            catch (PingException e)
            {
                if (e.InnerException == null)
                    return new CallResult<long>(0, new CantConnectError { Message = "Ping failed: " + e.Message });

                if (e.InnerException is SocketException exception)
                    return new CallResult<long>(0, new CantConnectError { Message = "Ping failed: " + exception.SocketErrorCode });
                return new CallResult<long>(0, new CantConnectError { Message = "Ping failed: " + e.InnerException.Message });
            }
            finally
            {
                ctRegistration.Dispose();
                ping.Dispose();
            }

            if (ct.IsCancellationRequested)
                return new CallResult<long>(0, new CancellationRequestedError());

            return reply.Status == IPStatus.Success ? new CallResult<long>(reply.RoundtripTime, null) : new CallResult<long>(0, new CantConnectError { Message = "Ping failed: " + reply.Status });
        }

        /// <summary>
        /// Execute a request to the uri and deserialize the response into the provided type parameter
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="uri">The uri to send the request to</param>
        /// <param name="method">The method of the request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <param name="parameters">The parameters of the request</param>
        /// <param name="signed">Whether or not the request should be authenticated</param>
        /// <param name="checkResult">Whether or not the resulting object should be checked for missing properties in the mapping (only outputs if log verbosity is Debug)</param>
        /// <param name="parameterPosition">Where the parameters should be placed, overwrites the value set in the client</param>
        /// <param name="arraySerialization">How array parameters should be serialized, overwrites the value set in the client</param>
        /// <param name="credits">Credits used for the request</param>
        /// <param name="deserializer">The JsonSerializer to use for deserialization</param>
        /// <param name="additionalHeaders">Additional headers to send with the request</param>
        /// <returns></returns>
        [return: NotNull]
        protected async Task<WebCallResult<T>> SendRequestAsync<T>(
            Uri uri,
            HttpMethod method,
            CancellationToken cancellationToken,
            Dictionary<string, object> parameters,
            bool signed = false,
            bool checkResult = true,
            HttpMethodParameterPosition? parameterPosition = null,
            ArrayParametersSerialization? arraySerialization = null,
            int credits = 1,
            JsonSerializer? deserializer = null,
            Dictionary<string, string>? additionalHeaders = null) where T : class
        {
            var requestId = NextId();
#if DEBUG
            ClientLog?.Debug($"[{requestId}] Creating request for " + uri);
#endif
            if (signed && authProvider == null)
            {
#if DEBUG
                ClientLog?.Warning($"[{requestId}] Request {uri.AbsolutePath} failed because no ApiCredentials were provided");
#endif
                return new WebCallResult<T>(null, null, null, new NoApiCredentialsError());
            }

            var paramsPosition = parameterPosition ?? ParameterPositions[method];
            var request = ConstructRequest(uri, method, parameters, signed, paramsPosition, arraySerialization ?? this.arraySerialization, requestId, additionalHeaders);
#if DEBUG
            string? paramString = "";
            if (paramsPosition == HttpMethodParameterPosition.InBody)
                paramString = " with request body " + request.Content;

            if (ClientLog?.LogLevel == LogLevel.Trace)
            {
                var headers = request.GetHeaders();
                if (headers.Any())
                    paramString += " with headers " + string.Join(", ", headers.Select(h => h.Key + $"=[{string.Join(",", h.Value)}]"));
            }

            ClientLog?.Debug($"[{requestId}] Sending {method}{(signed ? " signed" : "")} request to {request.Uri}{paramString ?? " "}{(apiProxy == null ? "" : $" via proxy {apiProxy.Host}")}");
#endif
            return await GetResponseAsync<T>(request, deserializer, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the request and returns the result deserialized into the type parameter class
        /// </summary>
        /// <param name="request">The request object to execute</param>
        /// <param name="deserializer">The JsonSerializer to use for deserialization</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        protected async Task<WebCallResult<T>> GetResponseAsync<T>(Request request, JsonSerializer? deserializer, CancellationToken cancellationToken)
        {
            try
            {
                TotalRequestsMade++;
                var response = await request.GetResponseAsync(cancellationToken).ConfigureAwait(false);
                var statusCode = response.StatusCode;
                var headers = response.ResponseHeaders;
                var responseStream = await response.GetResponseStreamAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    // If we have to manually parse error responses (can't rely on HttpStatusCode) we'll need to read the full
                    // response before being able to deserialize it into the resulting type since we don't know if it an error response or data
                    if (manualParseError)
                    {
                        using var reader = new StreamReader(responseStream);
                        var data = await reader.ReadToEndAsync().ConfigureAwait(false);
                        responseStream.Close();
                        response.Close();

                        // Validate if it is valid json. Sometimes other data will be returned, 502 error html pages for example
                        var parseResult = Json.ValidateJson(data);
                        if (!parseResult.Success)
                            return WebCallResult<T>.CreateErrorResult(response.StatusCode, response.ResponseHeaders, parseResult.Error!);

                        // Let the library implementation see if it is an error response, and if so parse the error
                        var error = await TryParseErrorAsync(parseResult.Data).ConfigureAwait(false);
                        if (error != null)
                            return WebCallResult<T>.CreateErrorResult(response.StatusCode, response.ResponseHeaders, error);

                        // Not an error, so continue deserializing
                        var deserializeResult = Json.Deserialize<T>(parseResult.Data, null, request.RequestId);
#if DEBUG
                        return new WebCallResult<T>(response.StatusCode, response.ResponseHeaders, Json.OutputOriginalData ? data : null, deserializeResult.Data, deserializeResult.Error);
#else
                        return new WebCallResult<T>(response.StatusCode, response.ResponseHeaders, null, deserializeResult.Data, deserializeResult.Error);
#endif
                    }
                    else
                    {
                        // Success status code, and we don't have to check for errors. Continue deserializing directly from the stream
                        var desResult = await DeserializeAsync<T>(responseStream, request.RequestId).ConfigureAwait(false);
                        responseStream.Close();
                        response.Close();
#if DEBUG
                        return new WebCallResult<T>(statusCode, headers, Json.OutputOriginalData ? desResult.OriginalData : null, desResult.Data, desResult.Error);
#else
                        return new WebCallResult<T>(statusCode, headers, null, desResult.Data, desResult.Error);
#endif
                    }
                }
                else
                {
                    // Http status code indicates error
                    using var reader = new StreamReader(responseStream);
                    var data = await reader.ReadToEndAsync().ConfigureAwait(false);
#if DEBUG
                    ClientLog?.Debug($"[{request.RequestId}] Error received: {data}");
#endif
                    responseStream.Close();
                    response.Close();
                    var parseResult = Json.ValidateJson(data);
                    var error = parseResult.Success ? ParseErrorResponse(parseResult.Data) : parseResult.Error!;
                    if (error.Code == null || error.Code == 0)
                        error.Code = (int)response.StatusCode;
                    return new WebCallResult<T>(statusCode, headers, default, error);
                }
            }
#if DEBUG
            catch (HttpRequestException requestException)
            {
                // Request exception, can't reach server for instance
                var exceptionInfo = requestException.ToLogString();

                ClientLog?.Warning($"[{request.RequestId}] Request exception: " + exceptionInfo);

                return new WebCallResult<T>(null, null, default, new WebError(exceptionInfo));
            }
            catch (OperationCanceledException canceledException)
            {
                if (canceledException.CancellationToken == cancellationToken)
                {
                    // Cancellation token cancelled by caller

                    ClientLog?.Warning($"[{request.RequestId}] Request cancel requested");

                    return new WebCallResult<T>(null, null, default, new CancellationRequestedError());
                }
                else
                {
                    // Request timed out

                    ClientLog?.Warning($"[{request.RequestId}] Request timed out");

                    return new WebCallResult<T>(null, null, default, new WebError($"[{request.RequestId}] Request timed out"));
                }
            }
#else
            catch (HttpRequestException)
            {
                return new WebCallResult<T>(null, null, default, null);
            }
#endif
        }

        /// <summary>
        /// Can be used to parse an error even though response status indicates success. Some apis always return 200 OK, even though there is an error.
        /// When setting manualParseError to true this method will be called for each response to be able to check if the response is an error or not.
        /// If the response is an error this method should return the parsed error, else it should return null
        /// </summary>
        /// <param name="data">Received data</param>
        /// <returns>Null if not an error, Error otherwise</returns>
        protected Task<ServerError?> TryParseErrorAsync(JToken data)
        {
            return Task.FromResult<ServerError?>(null);
        }

        /// <summary>
        /// Creates a request object
        /// </summary>
        /// <param name="uri">The uri to send the request to</param>
        /// <param name="method">The method of the request</param>
        /// <param name="parameters">The parameters of the request</param>
        /// <param name="signed">Whether or not the request should be authenticated</param>
        /// <param name="parameterPosition">Where the parameters should be placed</param>
        /// <param name="arraySerialization">How array parameters should be serialized</param>
        /// <param name="requestId">Unique id of a request</param>
        /// <param name="additionalHeaders">Additional headers to send with the request</param>
        /// <returns></returns>
        protected Request ConstructRequest(
            Uri uri,
            HttpMethod method,
            Dictionary<string, object> parameters,
            bool signed,
            HttpMethodParameterPosition parameterPosition,
            ArrayParametersSerialization arraySerialization,
            int requestId,
            Dictionary<string, string>? additionalHeaders)
        {
            var uriString = uri.ToString();
            if (authProvider != null)
                parameters = authProvider.AddAuthenticationToParameters(uriString, method, parameters, signed, parameterPosition, arraySerialization);

            if (parameterPosition == HttpMethodParameterPosition.InUri && parameters.Any() == true)
                uriString += "?" + parameters.CreateParamString(true, arraySerialization);

            var contentType = requestBodyFormat == RequestBodyFormat.Json ? Constants.JsonContentHeader : Constants.FormContentHeader;
            var request = RequestFactory.Create(method, uriString, requestId);
            request.Accept = Constants.JsonContentHeader;

            var headers = new Dictionary<string, string>();
            if (authProvider != null)
                headers = authProvider.AddAuthenticationToHeaders(uriString, method, parameters, signed, parameterPosition, arraySerialization);

            foreach (var header in headers)
                request.AddHeader(header.Key, header.Value);

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                    request.AddHeader(header.Key, header.Value);
            }

            if (StandardRequestHeaders != null)
            {
                foreach (var header in StandardRequestHeaders)
                    // Only add it if it isn't overwritten
                    if (additionalHeaders?.ContainsKey(header.Key) != true)
                        request.AddHeader(header.Key, header.Value);
            }

            if (parameterPosition == HttpMethodParameterPosition.InBody)
            {
                if (parameters.Any() == true)
                    WriteParamBody(request, parameters, contentType);
                else
                    request.SetContent(requestBodyEmptyContent, contentType);
            }

            return request;
        }

        /// <summary>
        /// Writes the parameters of the request to the request object body
        /// </summary>
        /// <param name="request">The request to set the parameters on</param>
        /// <param name="parameters">The parameters to set</param>
        /// <param name="contentType">The content type of the data</param>
        protected void WriteParamBody(Request request, Dictionary<string, object> parameters, string contentType)
        {
            if (requestBodyFormat == RequestBodyFormat.Json)
            {
                // Write the parameters as json in the body
                var stringData = JsonConvert.SerializeObject(parameters.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value));
                request.SetContent(stringData, contentType);
            }
            else if (requestBodyFormat == RequestBodyFormat.FormData)
            {
                // Write the parameters as form data in the body
                var formData = HttpUtility.ParseQueryString(string.Empty);
                foreach (var kvp in parameters.OrderBy(p => p.Key))
                {
                    if (kvp.Value.GetType().IsArray)
                    {
                        var array = (Array)kvp.Value;
                        foreach (var value in array)
                            formData.Add(kvp.Key, value.ToString());
                    }
                    else
                        formData.Add(kvp.Key, kvp.Value.ToString());
                }
                var stringData = formData.ToString();
                request.SetContent(stringData, contentType);
            }
        }

        /// <summary>
        /// Parse an error response from the server. Only used when server returns a status other than Success(200)
        /// </summary>
        /// <param name="error">The string the request returned</param>
        /// <returns></returns>
        protected Error ParseErrorResponse(JToken error)
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
                Logging.ClientLog?.Info("Time Is Out Of Sync, Attempting to Correct it");
                _ = ServerTimeClient.Guesser().ConfigureAwait(false);
                ServerTimeClient.CorrectionCount++;
            }
            return err;
        }
    }
}
