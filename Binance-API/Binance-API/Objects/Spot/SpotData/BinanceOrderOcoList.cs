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
using BinanceAPI.Enums;
using BinanceAPI.Objects.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.SpotData
{
    /// <summary>
    /// The result of placing a new OCO order
    /// </summary>
    public class BinanceOrderOcoList
    {
        /// <summary>
        /// The id of the order list
        /// </summary>
        public long OrderListId { get; set; }

        /// <summary>
        /// The contingency type
        /// </summary>
        public string ContingencyType { get; set; } = string.Empty;

        /// <summary>
        /// The order list status
        /// </summary>
        [JsonConverter(typeof(ListStatusTypeConverter))]
        public ListStatusType ListStatusType { get; set; }

        /// <summary>
        /// The order status
        /// </summary>
        [JsonConverter(typeof(ListOrderStatusConverter))]
        public ListOrderStatus ListOrderStatus { get; set; }

        /// <summary>
        /// The client id of the order list
        /// </summary>
        public string ListClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// The transaction time
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime TransactionTime { get; set; }

        /// <summary>
        /// The symbol of the order list
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The order in this list
        /// </summary>
        public IEnumerable<BinanceOrderId> Orders { get; set; } = Array.Empty<BinanceOrderId>();

        /// <summary>
        /// The order details
        /// </summary>
        [JsonOptionalProperty]
        public IEnumerable<BinancePlacedOcoOrder> OrderReports { get; set; } = Array.Empty<BinancePlacedOcoOrder>();
    }

    /// <summary>
    /// Order reference
    /// </summary>
    public class BinanceOrderId
    {
        /// <summary>
        /// The symbol of the order
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The id of the order
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// The client order id
        /// </summary>
        public string ClientOrderId { get; set; } = string.Empty;
    }

    /// <summary>
    /// The result of placing a new order
    /// </summary>
    public class BinancePlacedOcoOrder : BinanceOrderBase
    {
        /// <summary>
        /// The time the order was placed
        /// </summary>
        [JsonOptionalProperty]
        [JsonProperty("transactTime"), JsonConverter(typeof(TimestampConverter))]
        public new DateTime CreateTime { get; set; }
    }
}
