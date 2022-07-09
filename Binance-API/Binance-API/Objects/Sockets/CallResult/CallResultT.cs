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

using System.Diagnostics.CodeAnalysis;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// The result of an operation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CallResult<T> : CallResult
    {
        /// <summary>
        /// The data returned by the call, only available when Success = true
        /// </summary>
        [AllowNull]
        public T Data { get; internal set; }

        /// <summary>
        /// The original data returned by the call, only available when `OutputOriginalData` is set to `true` in the client options
        /// </summary>
        [AllowNull]
        public string OriginalData { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="error"></param>
        public CallResult([AllowNull] T data, [AllowNull] Error error) : base(error)
        {
            Data = data;
        }

        /// <summary>
        /// Overwrite bool check so we can use if(callResult) instead of if(callResult.Success)
        /// </summary>
        /// <param name="obj"></param>
        public static implicit operator bool(CallResult<T> obj)
        {
            return obj.Success == true;
        }

        /// <summary>
        /// Create an error result
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static WebCallResult<T> CreateErrorResult(Error error)
        {
            return new WebCallResult<T>(default, null, default, error);
        }
    }
}
