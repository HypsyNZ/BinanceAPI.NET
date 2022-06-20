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

using BinanceAPI.Interfaces;
using Newtonsoft.Json;

namespace BinanceAPI.Objects.Spot.MarketStream
{
    /// <summary>
    /// MiniTick info
    /// </summary>
    public abstract class BinanceMiniTick : BinanceStreamEvent
    {
        /// <summary>
        /// The symbol this data is for
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The current day close price. This is the latest price for this symbol.
        /// </summary>
        [JsonProperty("c")]
        public decimal LastPrice { get; set; }

        /// <summary>
        /// Todays open price
        /// </summary>
        [JsonProperty("o")]
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// Todays high price
        /// </summary>
        [JsonProperty("h")]
        public decimal HighPrice { get; set; }

        /// <summary>
        /// Todays low price
        /// </summary>
        [JsonProperty("l")]
        public decimal LowPrice { get; set; }

        /// <summary>
        /// Total traded volume
        /// </summary>
        public abstract decimal BaseVolume { get; set; }

        /// <summary>
        /// Total traded quote volume
        /// </summary>
        public abstract decimal QuoteVolume { get; set; }
    }

    /// <summary>
    /// Stream mini tick
    /// </summary>
    public class BinanceStreamMiniTick : BinanceMiniTick
    {
        /// <inheritdoc/>
        [JsonProperty("v")]
        public override decimal BaseVolume { get; set; }

        /// <inheritdoc/>
        [JsonProperty("q")]
        public override decimal QuoteVolume { get; set; }
    }
}
