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