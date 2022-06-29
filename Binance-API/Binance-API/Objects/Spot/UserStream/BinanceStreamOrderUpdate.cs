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

namespace BinanceAPI.Objects.Spot.UserStream
{
    /// <summary>
    /// Update data about an order
    /// </summary>
    public class BinanceStreamOrderUpdate : BinanceStreamEvent
    {
        /// <summary>
        /// The symbol the order is for
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The new client order id
        /// </summary>
        [JsonProperty("c")]
        public string ClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// The side of the order
        /// </summary>
        [JsonProperty("S"), JsonConverter(typeof(OrderSideConverter))]
        public OrderSide Side { get; set; }

        /// <summary>
        /// The type of the order
        /// </summary>
        [JsonProperty("o"), JsonConverter(typeof(OrderTypeConverter))]
        public OrderType Type { get; set; }

        /// <summary>
        /// The timespan the order is active
        /// </summary>
        [JsonProperty("f"), JsonConverter(typeof(TimeInForceConverter))]
        public TimeInForce TimeInForce { get; set; }

        /// <summary>
        /// The quantity of the order
        /// </summary>
        [JsonProperty("q")]
        public decimal Quantity { get; set; }

        /// <summary>
        /// The price of the order
        /// </summary>
        [JsonProperty("p")]
        public decimal Price { get; set; }

        /// <summary>
        /// The stop price of the order
        /// </summary>
        [JsonProperty("P")]
        public decimal StopPrice { get; set; }

        /// <summary>
        /// The trailing delta of the order
        /// </summary>
        [JsonProperty("d")]
        public int? TrailingDelta { get; set; }

        /// <summary>
        /// The iceberg quantity of the order
        /// </summary>
        [JsonProperty("F")]
        public decimal IcebergQuantity { get; set; }

        /// <summary>
        /// This id of the corresponding order list. (-1 if not part of an order list)
        /// </summary>
        [JsonProperty("g")]
        public int OrderListID { get; set; }

        /// <summary>
        /// The original client order id
        /// </summary>
        [JsonProperty("C")]
        public string? OriginalClientOrderId { get; set; } = string.Empty;

        /// <summary>
        /// The execution type
        /// </summary>
        [JsonProperty("x"), JsonConverter(typeof(ExecutionTypeConverter))]
        public ExecutionType ExecutionType { get; set; }

        /// <summary>
        /// The status of the order
        /// </summary>
        [JsonProperty("X"), JsonConverter(typeof(OrderStatusConverter))]
        public OrderStatus Status { get; set; }

        /// <summary>
        /// The reason the order was rejected
        /// </summary>
        [JsonProperty("r"), JsonConverter(typeof(OrderRejectReasonConverter))]
        public OrderRejectReason RejectReason { get; set; }

        /// <summary>
        /// The id of the order as assigned by Binance
        /// </summary>
        [JsonProperty("i")]
        public long OrderId { get; set; }

        /// <summary>
        /// The quantity of the last filled trade of this order
        /// </summary>
        [JsonProperty("l")]
        public decimal LastQuantityFilled { get; set; }

        /// <summary>
        /// <para>Cumulative filled quantity</para>
        /// The quantity of all trades that were filled for this order
        /// </summary>
        [JsonProperty("z")]
        public decimal QuantityFilled { get; set; }

        /// <summary>
        /// The price of the last filled trade
        /// </summary>
        [JsonProperty("L")]
        public decimal LastPriceFilled { get; set; }

        /// <summary>
        /// The commission payed
        /// </summary>
        [JsonProperty("n")]
        public decimal Commission { get; set; }

        /// <summary>
        /// The asset the commission was taken from
        /// </summary>
        [JsonProperty("N")]
        public string CommissionAsset { get; set; } = string.Empty;

        /// <summary>
        /// The time of the update
        /// </summary>
        [JsonProperty("T"), JsonConverter(typeof(TimestampConverter))]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// The trade id
        /// </summary>
        [JsonProperty("t")]
        public long TradeId { get; set; }

        /// <summary>
        /// Ignore
        /// </summary>
        [JsonProperty("I")]
        public long I { get; set; }

        /// <summary>
        /// Is working
        /// </summary>
        [JsonProperty("w")]
        public bool IsWorking { get; set; }

        /// <summary>
        /// Whether the buyer is the maker
        /// </summary>
        [JsonProperty("m")]
        public bool BuyerIsMaker { get; set; }

        /// <summary>
        /// Ignore
        /// </summary>
        [JsonProperty("M")]
        public bool M { get; set; }

        /// <summary>
        /// Time the order was created
        /// </summary>
        [JsonProperty("O"), JsonConverter(typeof(TimestampConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// <para>Average price can be found by doing <see href="QuoteQuantityFilled"/> divided by <see href="QuantityFilled"/>.</para>
        /// Cumulative quote asset transacted quantity
        /// </summary>
        [JsonProperty("Z")]
        public decimal QuoteQuantityFilled { get; set; }

        /// <summary>
        /// Last quote asset transacted quantity (i.e. LastPrice * LastQuantity)
        /// </summary>
        [JsonProperty("Y")]
        public decimal LastQuoteQuantity { get; set; }

        /// <summary>
        /// Quote order quantity
        /// </summary>
        [JsonProperty("Q")]
        public decimal QuoteQuantity { get; set; }
    }
}
