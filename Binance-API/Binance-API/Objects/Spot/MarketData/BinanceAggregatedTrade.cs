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

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// Compressed aggregated trade information. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
    /// </summary>
    public class BinanceAggregatedTrade
    {
        /// <summary>
        /// The id of this aggregation
        /// </summary>
        [JsonProperty("a")]
        public long AggregateTradeId { get; set; }

        /// <summary>
        /// The price of trades in this aggregation
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// The total quantity of trades in the aggregation
        /// </summary>
        [JsonProperty("q")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The first trade id in this aggregation
        /// </summary>
        [JsonProperty("f")]
        public long FirstTradeId { get; set; }

        /// <summary>
        /// The last trade id in this aggregation
        /// </summary>
        [JsonProperty("l")]
        public long LastTradeId { get; set; }

        /// <summary>
        /// The timestamp of the trades
        /// </summary>
        [JsonProperty("T"), JsonConverter(typeof(TimestampConverter))]
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// Whether the buyer was the maker
        /// </summary>
        [JsonProperty("m")]
        public bool BuyerIsMaker { get; set; }

        /// <summary>
        /// Whether the trade was matched at the best price
        /// </summary>
        [JsonProperty("M")]
        public bool WasBestPriceMatch { get; set; }
    }
}
