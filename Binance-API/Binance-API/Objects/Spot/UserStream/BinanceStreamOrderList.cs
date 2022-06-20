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
using BinanceAPI.Enums;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.UserStream
{
    /// <summary>
    /// Order list info
    /// </summary>
    public class BinanceStreamOrderList : BinanceStreamEvent
    {
        /// <summary>
        /// The id of the order list
        /// </summary>
        [JsonProperty("g")]
        public long OrderListId { get; set; }

        /// <summary>
        /// The contingency type
        /// </summary>
        [JsonProperty("c")]
        public string ContingencyType { get; set; } = string.Empty;

        /// <summary>
        /// The order list status
        /// </summary>
        [JsonConverter(typeof(ListStatusTypeConverter))]
        [JsonProperty("l")]
        public ListStatusType ListStatusType { get; set; }

        /// <summary>
        /// The order status
        /// </summary>
        [JsonConverter(typeof(ListOrderStatusConverter))]
        [JsonProperty("L")]
        public ListOrderStatus ListOrderStatus { get; set; }

        /// <summary>
        /// The client id of the order list
        /// </summary>
        [JsonProperty("C")]
        public string ListClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// The transaction time
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        [JsonProperty("T")]
        public DateTime TransactionTime { get; set; }

        /// <summary>
        /// The symbol of the order list
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The order in this list
        /// </summary>
        [JsonProperty("O")]
        public IEnumerable<BinanceStreamOrderId> Orders { get; set; } = Array.Empty<BinanceStreamOrderId>();
    }

    /// <summary>
    /// Order reference
    /// </summary>
    public class BinanceStreamOrderId
    {
        /// <summary>
        /// The symbol of the order
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The id of the order
        /// </summary>
        [JsonProperty("i")]
        public long OrderId { get; set; }

        /// <summary>
        /// The client order id
        /// </summary>
        [JsonProperty("c")]
        public string ClientOrderId { get; set; } = string.Empty;
    }
}
