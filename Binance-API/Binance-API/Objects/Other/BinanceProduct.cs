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

namespace BinanceAPI.Objects.Other
{
    /// <summary>
    /// Info on a product
    /// </summary>
    public class BinanceProduct
    {
        /// <summary>
        /// Name of the symbol
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Status of the symbol
        /// </summary>
        [JsonProperty("st")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Name of the base asset
        /// </summary>
        [JsonProperty("b")]
        public string BaseAsset { get; set; } = string.Empty;

        /// <summary>
        /// Name of the quote asset
        /// </summary>
        [JsonProperty("q")]
        public string QuoteAsset { get; set; } = string.Empty;

        /// <summary>
        /// Char of the base asset
        /// </summary>
        [JsonProperty("ba")]
        public string? BaseAssetChar { get; set; }

        /// <summary>
        /// Char of the quote asset
        /// </summary>
        [JsonProperty("qa")]
        public string? QuoteAssetChar { get; set; }

        /// <summary>
        /// Base asset name
        /// </summary>
        [JsonProperty("an")]
        public string BaseAssetName { get; set; } = string.Empty;

        /// <summary>
        /// Quote asset name
        /// </summary>
        [JsonProperty("qn")]
        public string QuoteAssetName { get; set; } = string.Empty;

        /// <summary>
        /// Open price
        /// </summary>
        [JsonProperty("o")]
        public decimal? Open { get; set; }

        /// <summary>
        /// High price
        /// </summary>
        [JsonProperty("h")]
        public decimal? High { get; set; }

        /// <summary>
        /// Low price
        /// </summary>
        [JsonProperty("l")]
        public decimal? Low { get; set; }

        /// <summary>
        /// Close price
        /// </summary>
        [JsonProperty("c")]
        public decimal? Close { get; set; }

        /// <summary>
        /// Base volume
        /// </summary>
        [JsonProperty("v")]
        public decimal BaseVolume { get; set; }

        /// <summary>
        /// Quote volume
        /// </summary>
        [JsonProperty("qv")]
        public decimal QuoteVolume { get; set; }

        /// <summary>
        /// Amount of coins in circulation
        /// </summary>
        [JsonProperty("cs")]
        public decimal? CirculatingSupply { get; set; }
    }
}
