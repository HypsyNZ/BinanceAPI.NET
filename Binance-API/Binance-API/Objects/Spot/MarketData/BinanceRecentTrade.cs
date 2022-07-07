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
    /// Recent trade info
    /// </summary>
    public abstract class BinanceRecentTrade
    {
        /// <summary>
        /// The id of the trade
        /// </summary>
        [JsonProperty("id")]
        public long OrderId { get; set; }

        /// <summary>
        /// The price of the trade
        /// </summary>
        public decimal Price { get; set; }

        /// <inheritdoc />
        public abstract decimal BaseQuantity { get; set; }

        /// <inheritdoc />
        public abstract decimal QuoteQuantity { get; set; }

        /// <summary>
        /// The timestamp of the trade
        /// </summary>
        [JsonProperty("Time"), JsonConverter(typeof(TimestampConverter))]
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// Whether the buyer is maker
        /// </summary>
        [JsonProperty("isBuyerMaker")]
        public bool BuyerIsMaker { get; set; }

        /// <summary>
        /// Whether the trade was made at the best match
        /// </summary>
        public bool IsBestMatch { get; set; }
    }

    /// <summary>
    /// Recent trade with quote quantity
    /// </summary>
    public class BinanceRecentTradeQuote : BinanceRecentTrade
    {
        /// <inheritdoc />
        [JsonProperty("quoteQty")]
        public override decimal QuoteQuantity { get; set; }

        /// <inheritdoc />
        [JsonProperty("qty")]
        public override decimal BaseQuantity { get; set; }
    }

    /// <summary>
    /// Recent trade with base quantity
    /// </summary>
    public class BinanceRecentTradeBase : BinanceRecentTrade
    {
        /// <inheritdoc />
        [JsonProperty("qty")]
        public override decimal QuoteQuantity { get; set; }

        /// <inheritdoc />
        [JsonProperty("baseQty")]
        public override decimal BaseQuantity { get; set; }
    }
}
