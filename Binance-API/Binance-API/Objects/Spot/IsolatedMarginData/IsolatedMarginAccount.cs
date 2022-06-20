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
using BinanceAPI.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.IsolatedMarginData
{
    /// <summary>
    /// Isolated margin account info
    /// </summary>
    public class BinanceIsolatedMarginAccount
    {
        /// <summary>
        /// Account assets
        /// </summary>
        public IEnumerable<BinanceIsolatedMarginAccountSymbol> Assets { get; set; } = Array.Empty<BinanceIsolatedMarginAccountSymbol>();

        /// <summary>
        /// Total btc asset
        /// </summary>
        public decimal TotalAssetOfBtc { get; set; }

        /// <summary>
        /// Total liability
        /// </summary>
        public decimal TotalLiabilityOfBtc { get; set; }

        /// <summary>
        /// Total net asset
        /// </summary>
        public decimal TotalNetAssetOfBtc { get; set; }
    }

    /// <summary>
    /// Isolated margin account symbol
    /// </summary>
    public class BinanceIsolatedMarginAccountSymbol
    {
        /// <summary>
        /// Base asset
        /// </summary>
        public BinanceIsolatedMarginAccountAsset BaseAsset { get; set; } = default!;

        /// <summary>
        /// Quote asset
        /// </summary>
        public BinanceIsolatedMarginAccountAsset QuoteAsset { get; set; } = default!;

        /// <summary>
        /// Symbol name
        /// </summary>
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Isolated created
        /// </summary>
        public bool IsolatedCreated { get; set; }

        /// <summary>
        /// The margin level
        /// </summary>
        public decimal MarginLevel { get; set; }

        /// <summary>
        /// Margin level status
        /// </summary>
        [JsonConverter(typeof(MarginLevelStatusConverter))]
        public MarginLevelStatus MarginLevelStatus { get; set; }

        /// <summary>
        /// Margin ratio
        /// </summary>
        public decimal MarginRatio { get; set; }

        /// <summary>
        /// Index price
        /// </summary>
        public decimal IndexPrice { get; set; }

        /// <summary>
        /// Liquidate price
        /// </summary>
        public decimal LiquidatePrice { get; set; }

        /// <summary>
        /// Liquidate rate
        /// </summary>
        public decimal LiquidateRate { get; set; }

        /// <summary>
        /// If trading is enabled
        /// </summary>
        public bool TradeEnabled { get; set; }

        /// <summary>
        /// Account is enabled
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// Isolated margin account asset
    /// </summary>
    public class BinanceIsolatedMarginAccountAsset
    {
        /// <summary>
        /// Asset name
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// If borrow is enabled
        /// </summary>
        public bool BorrowEnabled { get; set; }

        /// <summary>
        /// Borrowed
        /// </summary>
        public decimal Borrowed { get; set; }

        /// <summary>
        /// Free
        /// </summary>
        [JsonProperty("free")]
        public decimal Available { get; set; }

        /// <summary>
        /// Interest
        /// </summary>
        public decimal Interest { get; set; }

        /// <summary>
        /// Locked
        /// </summary>
        public decimal Locked { get; set; }

        /// <summary>
        /// Net asset
        /// </summary>
        public decimal NetAsset { get; set; }

        /// <summary>
        /// Net asset in btc
        /// </summary>
        public decimal NetAssetOfBtc { get; set; }

        /// <summary>
        /// Is repay enabled
        /// </summary>
        public bool RepayEnabled { get; set; }

        /// <summary>
        /// Total asset
        /// </summary>
        public decimal Total { get; set; }
    }
}
