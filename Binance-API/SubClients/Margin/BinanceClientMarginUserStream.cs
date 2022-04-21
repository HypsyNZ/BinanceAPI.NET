using BinanceAPI.Interfaces.SubClients;
using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.UserData;
using BinanceAPI.Requests;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.SubClients.Margin
{
    /// <summary>
    /// Margin user stream endpoints
    /// </summary>
    public class BinanceClientMarginUserStream : IBinanceClientUserStream
    {
        // Margin
        private const string getListenKeyEndpoint = "userDataStream";

        private const string keepListenKeyAliveEndpoint = "userDataStream";
        private const string closeListenKeyEndpoint = "userDataStream";

        private readonly BinanceClient _baseClient;

        internal BinanceClientMarginUserStream(BinanceClient baseClient)
        {
            _baseClient = baseClient;
        }

        #region Create a ListenKey

        /// <summary>
        /// Starts a user stream  for margin account by requesting a listen key.
        /// This listen key can be used in subsequent requests to BinanceSocketClient.Spot.SubscribeToUserDataUpdates.
        /// The stream will close after 60 minutes unless a keep alive is send.
        /// </summary>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Listen key</returns>
        public async Task<WebCallResult<string>> StartUserStreamAsync(CancellationToken ct = default)
        {
            var result = await _baseClient.SendRequestInternal<BinanceListenKey>(GetUri.New(_baseClient.BaseAddress, getListenKeyEndpoint, "sapi", "1"), HttpMethod.Post, ct).ConfigureAwait(false);
            return result.As(result.Data?.ListenKey!);
        }

        #endregion Create a ListenKey

        #region Ping/Keep-alive a ListenKey

        /// <summary>
        /// Sends a keep alive for the current user stream for margin account listen key to keep the stream from closing.
        /// Stream auto closes after 60 minutes if no keep alive is send. 30 minute interval for keep alive is recommended.
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<object>> KeepAliveUserStreamAsync(string listenKey, CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            var parameters = new Dictionary<string, object>
            {
                { "listenKey", listenKey },
            };

            return await _baseClient.SendRequestInternal<object>(GetUri.New(_baseClient.BaseAddress, keepListenKeyAliveEndpoint, "sapi", "1"), HttpMethod.Put, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Ping/Keep-alive a ListenKey

        #region Invalidate a ListenKey

        /// <summary>
        /// Close the user stream for margin account
        /// </summary>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<object>> StopUserStreamAsync(string listenKey, CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            var parameters = new Dictionary<string, object>
            {
                { "listenKey", listenKey }
            };

            return await _baseClient.SendRequestInternal<object>(GetUri.New(_baseClient.BaseAddress, closeListenKeyEndpoint, "sapi", "1"), HttpMethod.Delete, ct, parameters).ConfigureAwait(false);
        }

        #endregion Invalidate a ListenKey
    }
}