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

using BinanceAPI.Attributes;
using BinanceAPI.Converters;
using BinanceAPI.Objects.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.SpotData
{
    /// <summary>
    /// The result of placing a new order
    /// </summary>
    public class BinancePlacedOrder : BinanceOrderBase
    {
        /// <summary>
        /// The time the order was placed
        /// </summary>
        [JsonOptionalProperty]
        [JsonProperty("transactTime"), JsonConverter(typeof(TimestampConverter))]
        public new DateTime CreateTime { get; set; }

        /// <summary>
        /// Fills for the order
        /// </summary>
        [JsonOptionalProperty]
        public IEnumerable<BinanceOrderTrade>? Fills { get; set; }

        /// <summary>
        /// Only present if a margin trade happened
        /// </summary>
        [JsonOptionalProperty]
        public decimal? MarginBuyBorrowAmount { get; set; }

        /// <summary>
        /// Only present if a margin trade happened
        /// </summary>
        [JsonOptionalProperty]
        public string? MarginBuyBorrowAsset { get; set; }
    }
}
