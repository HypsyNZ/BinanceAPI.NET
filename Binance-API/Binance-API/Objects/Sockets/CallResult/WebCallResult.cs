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

using System.Collections.Generic;
using System.Net;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// The result of a request
    /// </summary>
    public class WebCallResult : CallResult
    {
        /// <summary>
        /// The status code of the response. Note that a OK status does not always indicate success, check the Success parameter for this.
        /// </summary>
        public HttpStatusCode? ResponseStatusCode { get; set; }

        /// <summary>
        /// The response headers
        /// </summary>
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>>? ResponseHeaders { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="responseHeaders">Response headers</param>
        /// <param name="error">Error</param>
        public WebCallResult(
            HttpStatusCode? code,
            IEnumerable<KeyValuePair<string, IEnumerable<string>>>? responseHeaders, Error? error) : base(error)
        {
            ResponseHeaders = responseHeaders;
            ResponseStatusCode = code;
        }

        /// <summary>
        /// Create an error result
        /// </summary>
        /// <param name="code">Status code</param>
        /// <param name="responseHeaders">Response headers</param>
        /// <param name="error">Error</param>
        /// <returns></returns>
        public static WebCallResult CreateErrorResult(HttpStatusCode? code, IEnumerable<KeyValuePair<string, IEnumerable<string>>>? responseHeaders, Error error)
        {
            return new WebCallResult(code, responseHeaders, error);
        }

        /// <summary>
        /// Create an error result
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static WebCallResult CreateErrorResult(WebCallResult result)
        {
            return new WebCallResult(result.ResponseStatusCode, result.ResponseHeaders, result.Error);
        }
    }
}
