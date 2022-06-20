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

namespace BinanceAPI.Objects.Spot.MarginData
{
    /// <summary>
    /// Isolated Interest rate history
    /// https://binance-docs.github.io/apidocs/spot/en/#query-isolated-margin-fee-data-user_data
    /// </summary>
    public class BinanceInterestIsolatedMarginData
    {
        /// <summary>
        /// The coin
        /// </summary>
        [JsonProperty("symbol")]
        public string Coin { get; set; } = string.Empty;

        /// <summary>
        /// Vip level
        /// </summary>
        [JsonProperty("vipLevel")]
        public int VipLevel { get; set; } = 0;

        /// <summary>
        /// Leverage Multiplier for this Symbol
        /// </summary>
        [JsonProperty("leverage")]
        public int Leverage { get; set; } = 0;

        /// <summary>
        /// Asset data for this Symbol
        /// </summary>
        [JsonProperty("data")]
        public IEnumerable<BinanceIsolatedInterestData> Data { get; set; } = Array.Empty<BinanceIsolatedInterestData>();
    }

    /// <summary>
    /// Isolated Margin Interest Data
    /// </summary>
    public class BinanceIsolatedInterestData
    {
        /// <summary>
        /// Symbol
        /// </summary>
        [JsonProperty("coin")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// Daily Interest
        /// </summary>
        [JsonProperty("dailyInterest")]
        public decimal DailyInterest { get; set; } = 0;

        /// <summary>
        /// Borrow Limit
        /// </summary>
        [JsonProperty("borrowLimit")]
        public decimal BorrowLimit { get; set; } = 0;
    }
}
