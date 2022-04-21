using System;

namespace BinanceAPI.Requests
{
    /// <summary>
    /// Uniform Resource Indentifier
    /// https://en.wikipedia.org/wiki/Uniform_Resource_Identifier
    /// </summary>
    internal static class GetUri
    {
        /// <summary>
        /// Create a new Uri to complete a Request
        /// </summary>
        /// <param name="baseAddress">Base Address for the Client</param>
        /// <param name="endpoint">API Endpoint</param>
        /// <param name="apiVersion">API Version</param>
        /// <param name="endpointVersion">API Endpoint Version</param>
        /// <returns></returns>
        internal static Uri New(string baseAddress, string endpoint, string apiVersion, string? endpointVersion = null)
        {
            var result = $"{baseAddress}{apiVersion}/";

            if (!string.IsNullOrEmpty(endpointVersion))
                result += $"v{endpointVersion}/";

            result += endpoint;
            return new Uri(result);
        }
    }
}