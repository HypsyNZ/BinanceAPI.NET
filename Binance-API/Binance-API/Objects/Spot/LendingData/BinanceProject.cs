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

namespace BinanceAPI.Objects.Spot.LendingData
{
    /// <summary>
    /// Binance project info
    /// </summary>
    public class BinanceProject
    {
        /// <summary>
        /// The asset
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Display priority
        /// </summary>
        public int DisplayPriority { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Interest per lot
        /// </summary>
        public decimal InterestPerLot { get; set; }

        /// <summary>
        /// Interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Lot size
        /// </summary>
        public decimal LotSize { get; set; }

        /// <summary>
        /// Lots low limit
        /// </summary>
        public int LotsLowLimit { get; set; }

        /// <summary>
        /// Lots purchased
        /// </summary>
        public int LotsPurchased { get; set; }

        /// <summary>
        /// Lots upper limit
        /// </summary>
        public int LotsUpLimit { get; set; }

        /// <summary>
        /// Max number of lots per user
        /// </summary>
        public int MaxLotsPerUser { get; set; }

        /// <summary>
        /// Needs know your customer
        /// </summary>
        public bool NeedsKYC { get; set; }

        /// <summary>
        /// Project id
        /// </summary>
        public string ProjectId { get; set; } = string.Empty;

        /// <summary>
        /// Project name
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Has area limitation
        /// </summary>
        public bool WithAreaLimitation { get; set; }
    }
}
