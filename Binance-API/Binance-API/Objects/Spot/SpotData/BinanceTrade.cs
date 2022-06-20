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

namespace BinanceAPI.Objects.Spot.SpotData
{
    /// <summary>
    /// Information about a trade
    /// </summary>
    public class BinanceTrade
    {
        /// <summary>
        /// The symbol the trade is for
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The id of the trade
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The order id the trade belongs to
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// Id of the order list this order belongs to
        /// </summary>
        public long? OrderListId { get; set; }

        /// <summary>
        /// The price of the trade
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The quantity of the trade
        /// </summary>
        [JsonProperty("qty")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The quote quantity of the trade
        /// </summary>
        [JsonProperty("quoteQty")]
        public decimal QuoteQuantity { get; set; }

        /// <summary>
        /// The commission paid for the trade
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// The asset the commission is paid in
        /// </summary>
        public string CommissionAsset { get; set; } = string.Empty;

        /// <summary>
        /// The time the trade was made
        /// </summary>
        [JsonProperty("time"), JsonConverter(typeof(TimestampConverter))]
        public DateTime TradeTime { get; set; }

        /// <summary>
        /// Whether account was the buyer in the trade
        /// </summary>
        public bool IsBuyer { get; set; }

        /// <summary>
        /// Whether account was the maker in the trade
        /// </summary>
        public bool IsMaker { get; set; }

        /// <summary>
        /// Whether trade was made with the best match
        /// </summary>
        public bool IsBestMatch { get; set; }
    }
}
