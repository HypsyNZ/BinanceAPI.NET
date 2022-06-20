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

namespace BinanceAPI.Objects.Spot.MarginData
{
    /// <summary>
    /// Margin pair info
    /// </summary>
    public class BinanceMarginPair
    {
        /// <summary>
        /// Base asset of the pair
        /// </summary>
        [JsonProperty("base")]
        public string BaseAsset { get; set; } = string.Empty;

        /// <summary>
        /// Quote asset of the pair
        /// </summary>
        [JsonProperty("quote")]
        public string QuoteAsset { get; set; } = string.Empty;

        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Is buying allowed
        /// </summary>
        public bool IsBuyAllowed { get; set; }

        /// <summary>
        /// Is selling allowed
        /// </summary>
        public bool IsSellAllowed { get; set; }

        /// <summary>
        /// Is margin trading
        /// </summary>
        public bool IsMarginTrade { get; set; }

        /// <summary>
        /// Symbol name
        /// </summary>
        public string Symbol { get; set; } = string.Empty;
    }
}
