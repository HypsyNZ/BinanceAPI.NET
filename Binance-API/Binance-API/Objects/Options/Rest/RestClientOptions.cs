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

using BinanceAPI.Authentication;
using System;
using System.Net.Http;

namespace BinanceAPI.Options
{
    /// <summary>
    /// Base for rest client options
    /// </summary>
    public class RestClientOptions : ClientOptions
    {
        /// <summary>
        /// The time the server has to respond to a request before timing out
        /// </summary>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Http client to use. If a HttpClient is provided in this property the RequestTimeout and Proxy options will be ignored in requests and should be set on the provided HttpClient instance
        /// </summary>
        public HttpClient? HttpClient { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress">The base address of the API</param>
        public RestClientOptions(string baseAddress) : base(baseAddress)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress">The base address of the API</param>
        /// <param name="httpClient">Shared http client instance</param>
        public RestClientOptions(HttpClient httpClient, string baseAddress) : base(baseAddress)
        {
            HttpClient = httpClient;
        }

        /// <summary>
        /// Create a copy of the options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Copy<T>() where T : RestClientOptions, new()
        {
            var copy = new T
            {
                BaseAddress = BaseAddress,
                LogLevel = LogLevel,
                Proxy = Proxy,
                RequestTimeout = RequestTimeout,
                HttpClient = HttpClient
            };

            if (ApiCredentials != null)
                copy.ApiCredentials = ApiCredentials.Copy();

            return copy;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}, RequestTimeout: {RequestTimeout:c}";
        }
    }
}
