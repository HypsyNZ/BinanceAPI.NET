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

namespace BinanceAPI.Objects.Spot.LendingData
{
    /// <summary>
    /// Customized fixed project position
    /// </summary>
    public class BinanceCustomizedFixedProjectPosition
    {
        /// <summary>
        /// Asset name
        /// </summary>
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Can transfer
        /// </summary>
        public bool CanTransfer { get; set; }

        /// <summary>
        /// Create timestamp
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime CreateTimestamp { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// End time
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Interest
        /// </summary>
        public decimal Interest { get; set; }

        /// <summary>
        /// Interest rate
        /// </summary>
        public decimal InterestRate { get; set; }

        /// <summary>
        /// Lot
        /// </summary>
        public int Lot { get; set; }

        /// <summary>
        /// Position id
        /// </summary>
        public int PositionId { get; set; }

        /// <summary>
        /// Principal
        /// </summary>
        public decimal Principal { get; set; }

        /// <summary>
        /// Project id
        /// </summary>
        public string ProjectId { get; set; } = string.Empty;

        /// <summary>
        /// Project name
        /// </summary>
        public string ProjectName { get; set; } = string.Empty;

        /// <summary>
        /// Time of purchase
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime PurchaseTime { get; set; }

        /// <summary>
        /// Redeem date
        /// </summary>
        public string RedeemDate { get; set; } = string.Empty;

        /// <summary>
        /// Start time
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [JsonConverter(typeof(ProjectStatusConverter))]
        public ProjectStatus Status { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [JsonConverter(typeof(ProjectTypeConverter))]
        public ProjectType Type { get; set; }
    }
}
