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

namespace BinanceAPI.Enums
{
    /// <summary>
    /// Filter type
    /// </summary>
    public enum SymbolFilterType
    {
        /// <summary>
        /// Unknown filter type
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Price filter
        /// </summary>
        Price = 1,

        /// <summary>
        /// Price percent filter
        /// </summary>
        PricePercent = 2,

        /// <summary>
        /// Lot size filter
        /// </summary>
        LotSize = 3,

        /// <summary>
        /// Market lot size filter
        /// </summary>
        MarketLotSize = 4,

        /// <summary>
        /// Min notional filter
        /// </summary>
        MinNotional = 5,

        /// <summary>
        /// Max orders filter
        /// </summary>
        MaxNumberOrders = 6,

        /// <summary>
        /// Max algo orders filter
        /// </summary>
        MaxNumberAlgorithmicOrders = 7,

        /// <summary>
        /// Max iceberg parts filter
        /// </summary>
        IcebergParts = 8,

        /// <summary>
        /// Max position filter
        /// </summary>
        MaxPosition = 9,

        /// <summary>
        /// Trailing delta filter
        /// </summary>,
        TrailingDelta = 10
    }
}
