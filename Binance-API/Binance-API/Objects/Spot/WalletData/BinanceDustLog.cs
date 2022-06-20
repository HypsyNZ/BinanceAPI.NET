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
    /// Dust log response details
    /// </summary>
    public class BinanceDustLogList
    {
        /// <summary>
        /// Total counts of exchange
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Rows
        /// </summary>
        public IEnumerable<BinanceDustLog> UserAssetDribblets { get; set; } = Array.Empty<BinanceDustLog>();
    }

    /// <summary>
    /// Dust log details
    /// </summary>
    public class BinanceDustLog
    {
        /// <summary>
        /// Total transferred
        /// </summary>
        [JsonProperty("totalTransferedAmount")]
        public decimal TransferredTotal { get; set; }

        /// <summary>
        /// Total service charge
        /// </summary>
        [JsonProperty("totalServiceChargeAmount")]
        public decimal ServiceChargeTotal { get; set; }

        /// <summary>
        /// Transaction id
        /// </summary>
        [JsonProperty("transId")]
        public long TransactionId { get; set; }

        /// <summary>
        /// Detail logs
        /// </summary>
        [JsonProperty("userAssetDribbletDetails")]
        public IEnumerable<BinanceDustLogDetails> Logs { get; set; } = Array.Empty<BinanceDustLogDetails>();

        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("operateTime")]
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime OperateTime { get; set; }
    }

    /// <summary>
    /// Dust log entry details
    /// </summary>
    public class BinanceDustLogDetails
    {
        /// <summary>
        /// Transaction id
        /// </summary>
        [JsonProperty("transId")]
        public long TransactionId { get; set; }

        /// <summary>
        /// Service charge
        /// </summary>
        [JsonProperty("serviceChargeAmount")]
        public decimal ServiceChargeAmount { get; set; }

        /// <summary>
        /// Amount
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        [JsonProperty("operateTime")]
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime OperateTime { get; set; }

        /// <summary>
        /// Transferred amount
        /// </summary>
        [JsonProperty("transferedAmount")]
        public decimal TransferredAmount { get; set; }

        /// <summary>
        /// Asset
        /// </summary>
        [JsonProperty("fromAsset")]
        public string FromAsset { get; set; } = string.Empty;
    }
}
