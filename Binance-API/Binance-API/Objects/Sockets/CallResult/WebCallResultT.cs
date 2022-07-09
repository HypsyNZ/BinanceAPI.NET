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
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// The result of a request
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebCallResult<T> : CallResult<T>
    {
        /// <summary>
        /// The status code of the response. Note that a OK status does not always indicate success, check the Success parameter for this.
        /// </summary>
        public HttpStatusCode ResponseStatusCode { get; set; }

        /// <summary>
        /// The response headers
        /// </summary>
        [AllowNull]
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> ResponseHeaders { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="responseHeaders"></param>
        /// <param name="data"></param>
        /// <param name="error"></param>
        public WebCallResult(HttpStatusCode code, [AllowNull] IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, [AllowNull] T data, [AllowNull] Error error) : base(data, error)
        {
            ResponseStatusCode = code;
            ResponseHeaders = responseHeaders;
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="originalData"></param>
        /// <param name="responseHeaders"></param>
        /// <param name="data"></param>
        /// <param name="error"></param>
        public WebCallResult(HttpStatusCode code, IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, [AllowNull] string originalData, [AllowNull] T data, Error error) : base(data, error)
        {
            OriginalData = originalData;
            ResponseStatusCode = code;
            ResponseHeaders = responseHeaders;
        }

        /// <summary>
        /// Copy the WebCallResult to a new data type
        /// </summary>
        /// <typeparam name="K">The new type</typeparam>
        /// <param name="data">The data of the new type</param>
        /// <returns></returns>
        public WebCallResult<K> As<K>([AllowNull] K data)
        {
            return new WebCallResult<K>(ResponseStatusCode, ResponseHeaders, OriginalData, data, Error);
        }

        /// <summary>
        /// Create an error result
        /// </summary>
        /// <param name="code"></param>
        /// <param name="responseHeaders"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static WebCallResult<T> CreateErrorResult(HttpStatusCode code, IEnumerable<KeyValuePair<string, IEnumerable<string>>> responseHeaders, Error error)
        {
            return new WebCallResult<T>(code, responseHeaders, default, error);
        }
    }
}
