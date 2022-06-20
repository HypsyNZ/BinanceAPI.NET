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

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// A filter for order placed on a symbol.
    /// </summary>
    [JsonConverter(typeof(SymbolFilterConverter))]
    public class BinanceSymbolFilter
    {
        /// <summary>
        /// The type of this filter
        /// </summary>
        public SymbolFilterType FilterType { get; set; }
    }

    /// <summary>
    /// Price filter
    /// </summary>
    public class BinanceSymbolPriceFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The minimal price the order can be for
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// The max price the order can be for
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// The tick size of the price. The price can not have more precision as this and can only be incremented in steps of this.
        /// </summary>
        public decimal TickSize { get; set; }
    }

    /// <summary>
    /// Price percentage filter
    /// </summary>
    public class BinanceSymbolPercentPriceFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The max factor the price can deviate up
        /// </summary>
        public decimal MultiplierUp { get; set; }

        /// <summary>
        /// The max factor the price can deviate down
        /// </summary>
        public decimal MultiplierDown { get; set; }

        /// <summary>
        /// The amount of minutes the average price of trades is calculated over. 0 means the last price is used
        /// </summary>
        public int AveragePriceMinutes { get; set; }
    }

    /// <summary>
    /// Lot size filter
    /// </summary>
    public class BinanceSymbolLotSizeFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The minimal quantity of an order
        /// </summary>
        public decimal MinQuantity { get; set; }

        /// <summary>
        /// The maximum quantity of an order
        /// </summary>
        public decimal MaxQuantity { get; set; }

        /// <summary>
        /// The tick size of the quantity. The quantity can not have more precision as this and can only be incremented in steps of this.
        /// </summary>
        public decimal StepSize { get; set; }
    }

    /// <summary>
    /// Market lot size filter
    /// </summary>
    public class BinanceSymbolMarketLotSizeFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The minimal quantity of an order
        /// </summary>
        public decimal MinQuantity { get; set; }

        /// <summary>
        /// The maximum quantity of an order
        /// </summary>
        public decimal MaxQuantity { get; set; }

        /// <summary>
        /// The tick size of the quantity. The quantity can not have more precision as this and can only be incremented in steps of this.
        /// </summary>
        public decimal StepSize { get; set; }
    }

    /// <summary>
    /// Min notional filter
    /// </summary>
    public class BinanceSymbolMinNotionalFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The minimal total size of an order. This is calculated by Price * Quantity.
        /// </summary>
        public decimal MinNotional { get; set; }

        /// <summary>
        /// Whether or not this filter is applied to market orders. If so the average trade price is used.
        /// </summary>
        public bool ApplyToMarketOrders { get; set; }

        /// <summary>
        /// The amount of minutes the average price of trades is calculated over for market orders. 0 means the last price is used
        /// </summary>
        public int AveragePriceMinutes { get; set; }
    }

    /// <summary>
    ///Max orders filter
    /// </summary>
    public class BinanceSymbolMaxOrdersFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The max number of orders for this symbol
        /// </summary>
        public int MaxNumberOrders { get; set; }
    }

    /// <summary>
    /// Max algo orders filter
    /// </summary>
    public class BinanceSymbolMaxAlgorithmicOrdersFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The max number of Algorithmic orders for this symbol
        /// </summary>
        public int MaxNumberAlgorithmicOrders { get; set; }
    }

    /// <summary>
    /// Max iceberg parts filter
    /// </summary>
    public class BinanceSymbolIcebergPartsFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The max parts of an iceberg order for this symbol.
        /// </summary>
        public int Limit { get; set; }
    }

    /// <summary>
    /// Max position filter
    /// </summary>
    public class BinanceSymbolMaxPositionFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The MaxPosition filter defines the allowed maximum position an account can have on the base asset of a symbol.
        /// </summary>
        public decimal MaxPosition { get; set; }
    }

    /// <summary>
    /// Trailing delta filter
    /// </summary>
    public class BinanceSymbolTrailingDeltaFilter : BinanceSymbolFilter
    {
        /// <summary>
        /// The MinTrailingAboveDelta filter defines the minimum amount in Basis Point or BIPS above the price to activate the order.
        /// </summary>
        public int MinTrailingAboveDelta { get; set; }

        /// <summary>
        /// The MaxTrailingAboveDelta filter defines the maximum amount in Basis Point or BIPS above the price to activate the order.
        /// </summary>
        public int MaxTrailingAboveDelta { get; set; }

        /// <summary>
        /// The MinTrailingBelowDelta filter defines the minimum amount in Basis Point or BIPS below the price to activate the order.
        /// </summary>
        public int MinTrailingBelowDelta { get; set; }

        /// <summary>
        /// The MaxTrailingBelowDelta filter defines the minimum amount in Basis Point or BIPS below the price to activate the order.
        /// </summary>
        public int MaxTrailingBelowDelta { get; set; }
    }
}
