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