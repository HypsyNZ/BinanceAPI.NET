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

namespace BinanceAPI.Objects.Spot.LendingData
{
    /// <summary>
    /// Flexible product position
    /// </summary>
    public class BinanceFlexibleProductPosition
    {
        /// <summary>
        /// Annual interest rate
        /// </summary>
        public decimal AnnualInterestRate { get; set; }

        /// <summary>
        /// Asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Average annual interest rate
        /// </summary>
        [JsonProperty("avgAnnualInterestRate")]
        public decimal AverageAnnualInterestRate { get; set; }

        /// <summary>
        /// Can redeem
        /// </summary>
        public bool CanRedeem { get; set; }

        /// <summary>
        /// Daily interest rate
        /// </summary>
        public decimal DailyInterestRate { get; set; }

        /// <summary>
        /// Amount free
        /// </summary>
        public decimal FreeAmount { get; set; }

        /// <summary>
        /// Amount frozen
        /// </summary>
        public decimal FreezeAmount { get; set; }

        /// <summary>
        /// Amount locked
        /// </summary>
        public decimal LockedAmount { get; set; }

        /// <summary>
        /// The product id
        /// </summary>
        public string ProductId { get; set; } = string.Empty;

        /// <summary>
        /// The product name
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Redeeming amount
        /// </summary>
        public decimal RedeemingAmount { get; set; }

        /// <summary>
        /// Amount purchased today
        /// </summary>
        public decimal TodayPurchasedAmount { get; set; }

        /// <summary>
        /// Total amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Total interest
        /// </summary>
        public decimal TotalInterest { get; set; }
    }
}
