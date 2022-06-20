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
using Newtonsoft.Json;
using System;

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// Kline for mark or index price
    /// </summary>
    [JsonConverter(typeof(ArrayConverter))]
    public class BinanceMarkIndexKline
    {
        /// <summary>
        /// The time this candlestick opened
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(TimestampConverter))]
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// The price at which this candlestick opened
        /// </summary>
        [ArrayProperty(1)]
        public decimal Open { get; set; }

        /// <summary>
        /// The highest price in this candlestick
        /// </summary>
        [ArrayProperty(2)]
        public decimal High { get; set; }

        /// <summary>
        /// The lowest price in this candlestick
        /// </summary>
        [ArrayProperty(3)]
        public decimal Low { get; set; }

        /// <summary>
        /// The price at which this candlestick closed
        /// </summary>
        [ArrayProperty(4)]
        public decimal Close { get; set; }

        [ArrayProperty(5)] internal string? Ignore1 { get; set; } = string.Empty;

        /// <summary>
        /// The close time of this candlestick
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(TimestampConverter))]
        public DateTime CloseTime { get; set; }

        [ArrayProperty(7)] internal string? Ignore2 { get; set; } = string.Empty;

        /// <summary>
        /// Number of basic data
        /// </summary>
        [ArrayProperty(8)]
        public int BasicDataCount { get; set; }
    }
}
