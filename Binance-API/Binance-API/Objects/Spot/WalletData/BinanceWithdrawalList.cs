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
    /// Information about a withdrawal
    /// </summary>
    public class BinanceWithdrawal
    {
        /// <summary>
        /// The id of the withdrawal
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// The time the withdrawal was applied for
        /// </summary>
        public DateTime ApplyTime { get; set; }

        /// <summary>
        /// The amount of the withdrawal
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The address the asset was withdrawn to
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Tag for the address
        /// </summary>
        public string AddressTag { get; set; } = string.Empty;

        /// <summary>
        /// The transaction id of the withdrawal
        /// </summary>
        [JsonProperty("txId")]
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Transaction fee for the withdrawal
        /// </summary>
        public decimal TransactionFee { get; set; }

        /// <summary>
        /// The asset that was withdrawn
        /// </summary>
        [JsonProperty("coin")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Network that was used
        /// </summary>
        public string Network { get; set; } = string.Empty;

        /// <summary>
        /// Confirm times for withdraw
        /// </summary>
        [JsonProperty("confirmNo")]
        public int? ConfirmTimes { get; set; }

        /// <summary>
        /// The status of the withdrawal
        /// </summary>
        [JsonConverter(typeof(WithdrawalStatusConverter))]
        public WithdrawalStatus Status { get; set; }

        /// <summary>
        /// Transfer type: 1 for internal transfer, 0 for external transfer
        /// </summary>
        [JsonConverter(typeof(WithdrawDepositTransferTypeConverter))]
        public WithdrawDepositTransferType TransferType { get; set; }
    }
}
