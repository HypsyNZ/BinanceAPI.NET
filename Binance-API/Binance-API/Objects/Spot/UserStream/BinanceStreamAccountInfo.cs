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

namespace BinanceAPI.Objects.Spot.UserStream
{
    /// <summary>
    /// Information about an account
    /// </summary>
    public class BinanceStreamAccountInfo : BinanceStreamEvent
    {
        /// <summary>
        /// Time of last account update
        /// </summary>
        [JsonProperty("u"), JsonConverter(typeof(TimestampConverter))]
        public DateTime Time { get; set; }

        /// <summary>
        /// Commission percentage to pay when making trades
        /// </summary>
        [JsonProperty("m")]
        public decimal MakerCommission { get; set; }

        /// <summary>
        /// Commission percentage to pay when taking trades
        /// </summary>
        [JsonProperty("t")]
        public decimal TakerCommission { get; set; }

        /// <summary>
        /// Commission percentage to buy when buying
        /// </summary>
        [JsonProperty("b")]
        public decimal BuyerCommission { get; set; }

        /// <summary>
        /// Commission percentage to buy when selling
        /// </summary>
        [JsonProperty("s")]
        public decimal SellerCommission { get; set; }

        /// <summary>
        /// Boolean indicating if this account can trade
        /// </summary>
        [JsonProperty("T")]
        public bool CanTrade { get; set; }

        /// <summary>
        /// Boolean indicating if this account can withdraw
        /// </summary>
        [JsonProperty("W")]
        public bool CanWithdraw { get; set; }

        /// <summary>
        /// Boolean indicating if this account can deposit
        /// </summary>
        [JsonProperty("D")]
        public bool CanDeposit { get; set; }

        /// <summary>
        /// Permissions types
        /// </summary>
        [JsonProperty("P", ItemConverterType = typeof(EnumConverter))]
        public IEnumerable<AccountType> Permissions { get; set; } = Array.Empty<AccountType>();

        /// <summary>
        /// List of assets with their current balances
        /// </summary>
        [JsonProperty("B")]
        public IEnumerable<BinanceStreamBalance> Balances { get; set; } = Array.Empty<BinanceStreamBalance>();
    }

    /// <summary>
    /// Information about an asset balance
    /// </summary>
    public class BinanceStreamBalance
    {
        /// <summary>
        /// The asset this balance is for
        /// </summary>
        [JsonProperty("a")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// The amount that isn't locked in a trade
        /// </summary>
        [JsonProperty("f")]
        public decimal Free { get; set; }

        /// <summary>
        /// The amount that is currently locked in a trade
        /// </summary>
        [JsonProperty("l")]
        public decimal Locked { get; set; }

        /// <summary>
        /// The total balance of this asset (Free + Locked)
        /// </summary>
        public decimal Total => Free + Locked;
    }
}
