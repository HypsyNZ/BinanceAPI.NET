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

using Newtonsoft.Json;

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// Symbol info
    /// </summary>
    [JsonObject("rateLimits")]
    public class BinanceRateLimit
    {
        /// <summary>
        /// The symbol
        /// </summary>
        [JsonProperty("rateLimitType")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Rate Limit Interval
        /// </summary>
        public string Interval { get; set; } = string.Empty;

        /// <summary>
        /// Rate Limit Interval Number
        /// </summary>
        public int IntervalNum { get; set; } = 0;

        /// <summary>
        /// Rate Limit
        /// </summary>
        public int Limit { get; set; } = 0;
    }
}
