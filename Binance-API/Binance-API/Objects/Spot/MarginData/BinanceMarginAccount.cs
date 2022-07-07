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

namespace BinanceAPI.Objects.Spot.MarginData
{
    /// <summary>
    /// Information about margin account
    /// </summary>
    public class BinanceMarginAccount
    {
        /// <summary>
        /// Boolean indicating if this account can borrow
        /// </summary>
        public bool BorrowEnabled { get; set; }

        /// <summary>
        /// Boolean indicating if this account can trade
        /// </summary>
        public bool TradeEnabled { get; set; }

        /// <summary>
        /// Boolean indicating if this account can transfer
        /// </summary>
        public bool TransferEnabled { get; set; }

        /// <summary>
        /// Aggregate level of margin
        /// </summary>
        public decimal MarginLevel { get; set; }

        /// <summary>
        /// Aggregate total balance as BTC
        /// </summary>
        public decimal TotalAssetOfBtc { get; set; }

        /// <summary>
        /// Aggregate total liability balance of BTC
        /// </summary>
        public decimal TotalLiabilityOfBtc { get; set; }

        /// <summary>
        /// Aggregate total available net balance of BTC
        /// </summary>
        public decimal TotalNetAssetOfBtc { get; set; }

        /// <summary>
        /// Balance list
        /// </summary>
        [JsonProperty("userAssets")]
        public IEnumerable<BinanceMarginBalance> Balances { get; set; } = Array.Empty<BinanceMarginBalance>();
    }

    /// <summary>
    /// Information about an asset balance
    /// </summary>
    public class BinanceMarginBalance
    {
        /// <summary>
        /// The asset this balance is for
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// The amount that was borrowed
        /// </summary>
        public decimal Borrowed { get; set; }

        /// <summary>
        /// The amount that isn't locked in a trade
        /// </summary>
        [JsonProperty("free")]
        public decimal Available { get; set; }

        /// <summary>
        /// Commission to need pay by borrowed
        /// </summary>
        public decimal Interest { get; set; }

        /// <summary>
        /// The amount that is currently locked in a trade
        /// </summary>
        public decimal Locked { get; set; }

        /// <summary>
        /// The amount that is netAsset
        /// </summary>
        public decimal NetAsset { get; set; }

        /// <summary>
        /// The total balance of this asset (Free + Locked)
        /// </summary>
        public decimal Total => Available + Locked;
    }
}
