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
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.LendingData
{
    /// <summary>
    /// Savings product
    /// </summary>
    public class BinanceSavingsProduct
    {
        /// <summary>
        /// The asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Average annual interest rate
        /// </summary>
        [JsonProperty("avgAnnualInterestRate")]
        public decimal AverageAnnualInterestRate { get; set; }

        /// <summary>
        /// Tier Average annual interest rate
        /// </summary>
        [JsonProperty("tierAnnualInterestRate")]
        public Dictionary<string, decimal> TierAnnualInterestRate { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// Can purchase
        /// </summary>
        public bool CanPurchase { get; set; }

        /// <summary>
        /// Can redeem
        /// </summary>
        public bool CanRedeem { get; set; }

        /// <summary>
        /// Daily interest per thousand
        /// </summary>
        public decimal DailyInterestPerThousand { get; set; }

        /// <summary>
        /// Is featured
        /// </summary>
        public bool Featured { get; set; }

        /// <summary>
        /// Minimal amount to purchase
        /// </summary>
        [JsonProperty("minPurchaseAmount")]
        public decimal MinimalPurchaseAmount { get; set; }

        /// <summary>
        /// Product id
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// Purchased amount
        /// </summary>
        public decimal PurchasedAmount { get; set; }

        /// <summary>
        /// Status of the product
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Upper limit
        /// </summary>
        [JsonProperty("upLimit")]
        public decimal UpperLimit { get; set; }

        /// <summary>
        /// Upper limit per user
        /// </summary>
        [JsonProperty("upLimitPerUser")]
        public decimal UpperLimitPerUser { get; set; }
    }
}
