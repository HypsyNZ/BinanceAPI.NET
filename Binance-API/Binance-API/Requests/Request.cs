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

using BinanceAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.Requests
{
    /// <summary>
    /// Request object
    /// </summary>
    public class Request
    {
        private readonly HttpRequestMessage request;
        private readonly HttpClient httpClient;

        /// <summary>
        /// Create request object for web request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="client"></param>
        /// <param name="requestId"></param>
        public Request(HttpRequestMessage request, HttpClient client, int requestId)
        {
            httpClient = client;
            this.request = request;
            RequestId = requestId;
        }

        /// <inheritdoc />
        public string? Content { get; private set; }

        /// <inheritdoc />
        public string Accept
        {
            set => request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(value));
        }

        /// <inheritdoc />
        public HttpMethod Method
        {
            get => request.Method;
            set => request.Method = value;
        }

        /// <inheritdoc />
        public Uri Uri => request.RequestUri;

        /// <inheritdoc />
        public int RequestId { get; }

        /// <inheritdoc />
        public void SetContent(string data, string contentType)
        {
            Content = data;
            request.Content = new StringContent(data, Encoding.UTF8, contentType);
        }

        /// <inheritdoc />
        public void AddHeader(string key, string value)
        {
            request.Headers.Add(key, value);
        }

        /// <inheritdoc />
        public Dictionary<string, IEnumerable<string>> GetHeaders()
        {
            return request.Headers.ToDictionary(h => h.Key, h => h.Value);
        }

        /// <inheritdoc />
        public void SetContent(byte[] data)
        {
            request.Content = new ByteArrayContent(data);
        }

        /// <inheritdoc />
        public async Task<Response> GetResponseAsync(CancellationToken cancellationToken)
        {
            return new Response(await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false));
        }
    }
}
