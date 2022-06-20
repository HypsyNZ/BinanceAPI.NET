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

namespace BinanceAPI.Objects.Spot.MarginData
{
    /// <summary>
    /// Margin account snapshot
    /// </summary>
    public class BinanceMarginAccountSnapshot
    {
        /// <summary>
        /// Timestamp of the data
        /// </summary>
        [JsonConverter(typeof(TimestampConverter)), JsonProperty("updateTime")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Account type the data is for
        /// </summary>
        [JsonConverter(typeof(AccountTypeConverter))]
        public AccountType Type { get; set; }

        /// <summary>
        /// Snapshot data
        /// </summary>
        [JsonProperty("data")]
        public BinanceMarginAccountSnapshotData Data { get; set; } = default!;
    }

    /// <summary>
    /// Margin snapshot data
    /// </summary>
    public class BinanceMarginAccountSnapshotData
    {
        /// <summary>
        /// The margin level
        /// </summary>
        public decimal MarginLevel { get; set; }

        /// <summary>
        /// Total BTC asset
        /// </summary>
        public decimal TotalAssetOfBtc { get; set; }

        /// <summary>
        /// Total BTC liability
        /// </summary>
        public decimal TotalLiabilityOfBtc { get; set; }

        /// <summary>
        /// Total net BTC asset
        /// </summary>
        public decimal TotalNetAssetOfBtc { get; set; }

        /// <summary>
        /// Assets
        /// </summary>
        public IEnumerable<BinanceMarginBalance> UserAssets { get; set; } = Array.Empty<BinanceMarginBalance>();
    }
}
