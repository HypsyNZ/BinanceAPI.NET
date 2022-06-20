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
using BinanceAPI.Interfaces;
using Newtonsoft.Json;
using System;

namespace BinanceAPI.Objects.Spot.MarketStream
{
    /// <summary>
    /// Aggregated information about trades for a symbol
    /// </summary>
    public class BinanceStreamTrade : BinanceStreamEvent
    {
        /// <summary>
        /// The symbol the trade was for
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The id of this aggregated trade
        /// </summary>
        [JsonProperty("t")]
        public long OrderId { get; set; }

        /// <summary>
        /// The price of the trades
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// The combined quantity of the trades
        /// </summary>
        [JsonProperty("q")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The first trade id in this aggregation
        /// </summary>
        [JsonProperty("b")]
        public long BuyerOrderId { get; set; }

        /// <summary>
        /// The last trade id in this aggregation
        /// </summary>
        [JsonProperty("a")]
        public long SellerOrderId { get; set; }

        /// <summary>
        /// The time of the trades
        /// </summary>
        [JsonProperty("T"), JsonConverter(typeof(TimestampConverter))]
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// Whether the buyer was the maker
        /// </summary>
        [JsonProperty("m")]
        public bool BuyerIsMaker { get; set; }

        /// <summary>
        /// Unused
        /// </summary>
        [JsonProperty("M")]
        public bool IsBestMatch { get; set; }
    }
}
