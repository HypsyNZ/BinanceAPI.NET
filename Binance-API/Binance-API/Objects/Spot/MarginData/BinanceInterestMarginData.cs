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

namespace BinanceAPI.Objects.Spot.MarginData
{
    /// <summary>
    /// Interest rate history
    /// </summary>
    public class BinanceInterestMarginData
    {
        /// <summary>
        /// Vip level
        /// </summary>
        [JsonProperty("vipLevel")]
        public string VipLevel { get; set; } = string.Empty;

        /// <summary>
        /// The coin
        /// </summary>
        [JsonProperty("coin")]
        public string Coin { get; set; } = string.Empty;

        /// <summary>
        /// If coin can be transferred into cross
        /// </summary>
        [JsonProperty("transferIn")]
        public bool TransferIn { get; set; } = false;

        /// <summary>
        /// If coin can be borrowed in cross
        /// </summary>
        [JsonProperty("borrowable")]
        public bool Borrowable { get; set; } = false;

        /// <summary>
        /// The daily interest
        /// </summary>
        [JsonProperty("dailyInterest")]
        public decimal DailyInterest { get; set; }

        /// <summary>
        /// The yearly interest
        /// </summary>
        [JsonProperty("yearlyInterest")]
        public decimal YearlyInterest { get; set; }

        /// <summary>
        /// The yearly interest
        /// </summary>
        [JsonProperty("borrowLimit")]
        public decimal BorrowLimit { get; set; }

        /// <summary>
        /// Cross marginable pairs for this coin
        /// </summary>
        [JsonProperty("marginablePairs")]
        public string[]? MarginablePairs { get; set; }
    }
}
