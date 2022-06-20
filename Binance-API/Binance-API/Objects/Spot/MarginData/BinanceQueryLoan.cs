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

namespace BinanceAPI.Objects.Spot.MarginData
{
    /// <summary>
    /// Loan info
    /// </summary>
    public class BinanceLoan
    {
        /// <summary>
        /// The asset of the loan
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// The transaction id of the loan
        /// </summary>
        [JsonProperty("txId")]
        public long TransactionId { get; set; }

        /// <summary>
        /// Principal repaid
        /// </summary>
        public decimal Principal { get; set; }

        /// <summary>
        /// Time of repay completed
        /// </summary>
        [JsonProperty("timestamp"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Time { get; set; }

        /// <summary>
        /// The status of the loan
        /// </summary>
        [JsonConverter(typeof(MarginStatusConverter))]
        public MarginStatus Status { get; set; }
    }
}
