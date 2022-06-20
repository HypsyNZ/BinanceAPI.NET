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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.WalletData
{
    /// <summary>
    /// Result of dust transfer
    /// </summary>
    public class BinanceDustTransferResult
    {
        /// <summary>
        /// Total service charge
        /// </summary>
        [JsonProperty("totalServiceCharge")]
        public decimal TotalServiceCharge { get; set; }

        /// <summary>
        /// Total transferred
        /// </summary>
        [JsonProperty("totalTransfered")]
        public decimal TotalTransferred { get; set; }

        /// <summary>
        /// Transfer entries
        /// </summary>
        public IEnumerable<BinanceDustTransferResultEntry> TransferResult { get; set; } = Array.Empty<BinanceDustTransferResultEntry>();
    }

    /// <summary>
    /// Dust transfer entry
    /// </summary>
    public class BinanceDustTransferResultEntry
    {
        /// <summary>
        /// Amount of dust
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Asset
        /// </summary>
        public string FromAsset { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of conversion
        /// </summary>
        [JsonConverter(typeof(TimestampConverter)), JsonProperty("operateTime")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Service charge
        /// </summary>
        public decimal ServiceChargeAmount { get; set; }

        /// <summary>
        /// Transaction id
        /// </summary>
        [JsonProperty("tranId")]
        public long TransactionId { get; set; }

        /// <summary>
        /// BNB result amount
        /// </summary>
        [JsonProperty("TransferedAmount")]
        public decimal TransferredAmount { get; set; }
    }
}
