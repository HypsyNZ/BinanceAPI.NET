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

namespace BinanceAPI.Requests
{
    /// <summary>
    /// Uniform Resource Indentifier
    /// https://en.wikipedia.org/wiki/Uniform_Resource_Identifier
    /// </summary>
    internal static class GetUriString
    {
        /// <summary>
        /// Create a new Uri to complete a Request
        /// </summary>
        /// <param name="endpoint">API Endpoint</param>
        /// <param name="apiVersion">API Version</param>
        /// <param name="endpointVersion">API Endpoint Version</param>
        /// <returns></returns>
        internal static string Combine(string endpoint, string apiVersion, string? endpointVersion = null)
        {
            var result = $"{apiVersion}/";

            if (!string.IsNullOrEmpty(endpointVersion))
                result += $"v{endpointVersion}/";

            result += endpoint;
            return result;
        }
    }
}
