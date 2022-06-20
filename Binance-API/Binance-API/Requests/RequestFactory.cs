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
using BinanceAPI.Objects;
using System;
using System.Net;
using System.Net.Http;

namespace BinanceAPI.Requests
{
    /// <summary>
    /// WebRequest factory
    /// </summary>
    public class RequestFactory
    {
        private HttpClient httpClient = new();

        /// <inheritdoc />
        public void Configure(TimeSpan requestTimeout, ApiProxy? proxy, HttpClient? client = null)
        {
            if (client == null)
            {
                HttpMessageHandler handler = new HttpClientHandler()
                {
                    Proxy = proxy == null ? null : new WebProxy
                    {
                        Address = new Uri($"{proxy.Host}:{proxy.Port}"),
                        Credentials = proxy.Password == null ? null : new NetworkCredential(proxy.Login, proxy.Password)
                    }
                };

                httpClient = new HttpClient(handler) { Timeout = requestTimeout };
            }
            else
            {
                httpClient = client;
            }
        }

        /// <inheritdoc />
        public Request Create(HttpMethod method, string uri, int requestId)
        {
            return new Request(new HttpRequestMessage(method, uri), httpClient, requestId);
        }
    }
}
