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
    public class RequestFactory : IRequestFactory
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
        public IRequest Create(HttpMethod method, string uri, int requestId)
        {
            return new Request(new HttpRequestMessage(method, uri), httpClient, requestId);
        }
    }
}