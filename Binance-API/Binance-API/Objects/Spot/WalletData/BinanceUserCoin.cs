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

namespace BinanceAPI.Objects.Spot.WalletData
{
    /// <summary>
    /// Information about a coin for a user
    /// </summary>
    public class BinanceUserCoin
    {
        /// <summary>
        /// Coin code
        /// </summary>
        public string Coin { get; set; } = string.Empty;

        /// <summary>
        /// Deposit all is enabled
        /// </summary>
        public bool DepositAllEnable { get; set; }

        /// <summary>
        /// Amount free
        /// </summary>
        public decimal Free { get; set; }

        /// <summary>
        /// Amount frozen
        /// </summary>
        public decimal Freeze { get; set; }

        /// <summary>
        /// Ipo-able
        /// </summary>
        public decimal Ipoable { get; set; }

        /// <summary>
        /// Ipo-ing
        /// </summary>
        public decimal Ipoing { get; set; }

        /// <summary>
        /// Is the coin legally money
        /// </summary>
        public bool IsLegalMoney { get; set; }

        /// <summary>
        /// Amount locked
        /// </summary>
        public decimal Locked { get; set; }

        /// <summary>
        /// Storage
        /// </summary>
        public decimal Storage { get; set; }

        /// <summary>
        /// Is trading
        /// </summary>
        public bool Trading { get; set; }

        /// <summary>
        /// Withdraw all enabled
        /// </summary>
        public bool WithdrawAllEnable { get; set; }

        /// <summary>
        /// Name of the coin
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Currently withdrawing
        /// </summary>
        public decimal Withdrawing { get; set; }

        /// <summary>
        /// Networks
        /// </summary>
        public IEnumerable<BinanceNetwork> NetworkList { get; set; } = Array.Empty<BinanceNetwork>();
    }

    /// <summary>
    /// Network for a coin
    /// </summary>
    public class BinanceNetwork
    {
        /// <summary>
        /// Regex for an address on the network
        /// </summary>
        public string AddressRegex { get; set; } = string.Empty;

        /// <summary>
        /// Coin name
        /// </summary>
        public string Coin { get; set; } = string.Empty;

        /// <summary>
        /// Deposit description
        /// </summary>
        [JsonProperty("depositDesc")]
        public string DepositDescription { get; set; } = string.Empty;

        /// <summary>
        /// Deposit enabled
        /// </summary>
        [JsonProperty("depositEnable")]
        public bool DepositEnabled { get; set; }

        /// <summary>
        /// Is default network
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Regex for a memo
        /// </summary>
        public string MemoRegex { get; set; } = string.Empty;

        /// <summary>
        /// Minimal confirmations for balance confirmation
        /// </summary>
        [JsonProperty("minConfirm")]
        public int MinConfirmations { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Network
        /// </summary>
        public string Network { get; set; } = string.Empty;

        /// <summary>
        /// Reset address status
        /// </summary>
        public bool ResetAddressStatus { get; set; }

        /// <summary>
        /// Tips
        /// </summary>
        public string SpecialTips { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation number for balance unlock
        /// </summary>
        public int UnlockConfirm { get; set; }

        /// <summary>
        /// Withdraw description
        /// </summary>
        [JsonProperty("withdrawDesc")]
        public string WithdrawDescription { get; set; } = string.Empty;

        /// <summary>
        /// Withdraw is enabled
        /// </summary>
        [JsonProperty("withdrawEnable")]
        public bool WithdrawEnabled { get; set; }

        /// <summary>
        /// Fee for withdrawing
        /// </summary>
        public decimal WithdrawFee { get; set; }

        /// <summary>
        /// Minimal withdraw amount
        /// </summary>
        public decimal WithdrawMin { get; set; }

        /// <summary>
        /// Min withdraw step
        /// </summary>
        public decimal WithdrawIntegerMultiple { get; set; }

        /// <summary>
        /// Max withdraw amount
        /// </summary>
        public decimal WithdrawMax { get; set; }

        /// <summary>
        /// If the coin needs to provide memo to withdraw
        /// </summary>
        public bool SameAddress { get; set; }
    }
}
