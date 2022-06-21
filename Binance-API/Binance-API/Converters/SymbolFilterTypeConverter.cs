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

using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class SymbolFilterTypeConverter : BaseConverter<SymbolFilterType>
    {
        public SymbolFilterTypeConverter() : this(true)
        {
        }

        public SymbolFilterTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<SymbolFilterType, string>> Mapping => new()
        {
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.LotSize, "LOT_SIZE"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.MarketLotSize, "MARKET_LOT_SIZE"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.MinNotional, "MIN_NOTIONAL"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.Notional, "NOTIONAL"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.Price, "PRICE_FILTER"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.PricePercent, "PERCENT_PRICE"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.IcebergParts, "ICEBERG_PARTS"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.MaxNumberOrders, "MAX_NUM_ORDERS"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.MaxNumberAlgorithmicOrders, "MAX_NUM_ALGO_ORDERS"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.MaxPosition, "MAX_POSITION"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.TrailingDelta, "TRAILING_DELTA"),
            new KeyValuePair<SymbolFilterType, string>(SymbolFilterType.IcebergOrders, "EXCHANGE_MAX_NUM_ICEBERG_ORDERS"),
        };
    }
}
