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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.LendingData
{
    /// <summary>
    /// Lending account
    /// </summary>
    public class BinanceLendingAccount
    {
        /// <summary>
        /// Total amount in btc
        /// </summary>
        public decimal TotalAmountInBTC { get; set; }

        /// <summary>
        /// Total amount in usdt
        /// </summary>
        public decimal TotalAmountInUSDT { get; set; }

        /// <summary>
        /// Total fixed amount in btc
        /// </summary>
        public decimal TotalFixedAmountInBTC { get; set; }

        /// <summary>
        /// Total fixed amount in usdt
        /// </summary>
        public decimal TotalFixedAmountInUSDT { get; set; }

        /// <summary>
        /// Total flexible in btc
        /// </summary>
        public decimal TotalFlexibleInBTC { get; set; }

        /// <summary>
        /// Total flexible in usdt
        /// </summary>
        public decimal TotalFlexibleInUSDT { get; set; }

        /// <summary>
        /// Position amounts
        /// </summary>
        [JsonProperty("positionAmountVos")]
        public IEnumerable<BinanceLendingPositionAmount> PositionAmounts { get; set; } = Array.Empty<BinanceLendingPositionAmount>();
    }

    /// <summary>
    /// Lending position amount
    /// </summary>
    public class BinanceLendingPositionAmount
    {
        /// <summary>
        /// Amount of the asset
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Amount in btc
        /// </summary>
        public decimal AmountInBTC { get; set; }

        /// <summary>
        /// Amount in usdt
        /// </summary>
        public decimal AmountInUSDT { get; set; }

        /// <summary>
        /// Asset name
        /// </summary>
        public string Asset { get; set; } = string.Empty;
    }
}
