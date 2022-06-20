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

using BinanceAPI.Converters;
using BinanceAPI.Objects.Shared;
using Newtonsoft.Json;

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// Candlestick information for symbol
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BinanceSpotKline : BinanceKlineBase
    {
        /// <summary>
        /// The volume traded during this candlestick
        /// </summary>
        [ArrayProperty(5)]
        public override decimal BaseVolume { get; set; }

        /// <summary>
        /// The volume traded during this candlestick in the asset form
        /// </summary>
        [ArrayProperty(7)]
        public override decimal QuoteVolume { get; set; }

        /// <summary>
        /// Taker buy base asset volume
        /// </summary>
        [ArrayProperty(9)]
        public override decimal TakerBuyBaseVolume { get; set; }

        /// <summary>
        /// Taker buy quote asset volume
        /// </summary>
        [ArrayProperty(10)]
        public override decimal TakerBuyQuoteVolume { get; set; }

        /// <summary>
        /// Ignore
        /// </summary>
        [ArrayProperty(11)]
        public decimal? Ignore { get; set; }
    }
}
