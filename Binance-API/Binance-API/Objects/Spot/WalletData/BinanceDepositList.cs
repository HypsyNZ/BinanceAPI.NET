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

namespace BinanceAPI.Objects.Spot.WalletData
{
    /// <summary>
    /// Information about a deposit
    /// </summary>
    public class BinanceDeposit
    {
        /// <summary>
        /// Time the deposit was added to Binance
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime InsertTime { get; set; }

        /// <summary>
        /// The amount deposited
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The coin deposited
        /// </summary>
        public string Coin { get; set; } = string.Empty;

        /// <summary>
        /// The address of the deposit
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// The tag of the address of the deposit
        /// </summary>
        public string Tag { get; set; } = string.Empty;

        /// <summary>
        /// The network
        /// </summary>
        public string Network { get; set; } = string.Empty;

        /// <summary>
        /// The transaction id
        /// </summary>
        [JsonProperty("txId")]
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// The status of the deposit
        /// </summary>
        [JsonConverter(typeof(DepositStatusConverter))]
        public DepositStatus Status { get; set; }

        /// <summary>
        /// The transfer type
        /// </summary>
        [JsonConverter(typeof(WithdrawDepositTransferTypeConverter))]
        public WithdrawDepositTransferType TransferType { get; set; }

        /// <summary>
        /// Confirmations
        /// </summary>
        [JsonProperty("confirmTimes")]
        public string Confirmations { get; set; } = string.Empty;

        /// <summary>
        /// Network confirmations for unlocking
        /// </summary>
        [JsonProperty("unlockConfirm")]
        public string ConfirmationsForUnlock { get; set; } = string.Empty;
    }
}
