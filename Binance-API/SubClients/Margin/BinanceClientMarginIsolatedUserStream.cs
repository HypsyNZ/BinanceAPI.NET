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
    public class BinanceClientIsolatedMarginUserStream
    {
        // Margin
        private const string getListenKeyEndpoint = "userDataStream/isolated";

        private const string keepListenKeyAliveEndpoint = "userDataStream/isolated";
        private const string closeListenKeyEndpoint = "userDataStream/isolated";

        private readonly BinanceClientHost _baseClient;

        internal BinanceClientIsolatedMarginUserStream(BinanceClientHost baseClient)
        {
            _baseClient = baseClient;
        }

        #region Create a ListenKey

        /// <summary>
        /// Starts a user stream  for margin account by requesting a listen key.
        /// This listen key can be used in subsequent requests to BinanceSocketClient.Spot.SubscribeToUserDataUpdates
        /// The stream will close after 60 minutes unless a keep alive is send.
        /// </summary>
        /// <param name="symbol">The isolated symbol</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Listen key</returns>
        public async Task<WebCallResult<string>> StartIsolatedMarginUserStreamAsync(string symbol, CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"symbol", symbol}
            };

            var result = await _baseClient.SendRequestInternal<BinanceListenKey>(UriClient.GetBaseAddress() + GetUriString.Combine(getListenKeyEndpoint, "sapi", "1"), HttpMethod.Post, ct, parameters).ConfigureAwait(false);
            return result.As(result.Data?.ListenKey!);
        }

        #endregion Create a ListenKey

        #region Ping/Keep-alive a ListenKey

        /// <summary>
        /// Sends a keep alive for the current user stream for margin account listen key to keep the stream from closing.
        /// Stream auto closes after 60 minutes if no keep alive is send. 30 minute interval for keep alive is recommended.
        /// </summary>
        /// <param name="symbol">The isolated symbol</param>
        /// <param name="listenKey">The listen key to keep alive</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<object>> KeepAliveIsolatedMarginUserStreamAsync(string symbol, string listenKey, CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            var parameters = new Dictionary<string, object>
            {
                { "listenKey", listenKey },
                {"symbol", symbol}
            };

            return await _baseClient.SendRequestInternal<object>(UriClient.GetBaseAddress() + GetUriString.Combine(keepListenKeyAliveEndpoint, "sapi", "1"), HttpMethod.Put, ct, parameters, true).ConfigureAwait(false);
        }

        #endregion Ping/Keep-alive a ListenKey

        #region Invalidate a ListenKey

        /// <summary>
        /// Close the user stream for margin account
        /// </summary>
        /// <param name="symbol">The isolated symbol</param>
        /// <param name="listenKey">The listen key to invalidate</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns></returns>
        public async Task<WebCallResult<object>> CloseIsolatedMarginUserStreamAsync(string symbol, string listenKey, CancellationToken ct = default)
        {
            listenKey.ValidateNotNull(nameof(listenKey));

            var parameters = new Dictionary<string, object>
            {
                { "listenKey", listenKey },
                {"symbol", symbol}
            };

            return await _baseClient.SendRequestInternal<object>(UriClient.GetBaseAddress() + GetUriString.Combine(closeListenKeyEndpoint, "sapi", "1"), HttpMethod.Delete, ct, parameters).ConfigureAwait(false);
        }

        #endregion Invalidate a ListenKey
    }
}
