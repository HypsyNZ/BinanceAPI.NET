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

namespace BinanceAPI.Objects.Spot.WalletData
{
    internal class BinanceTradingStatusWrapper
    {
        public BinanceTradingStatus? Data { get; set; }
    }

    /// <summary>
    /// Trade status
    /// </summary>
    public class BinanceTradingStatus
    {
        /// <summary>
        /// Is locked
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Planned time of recovery
        /// </summary>
        public int PlannedRecoverTime { get; set; }

        /// <summary>
        /// Conditions
        /// </summary>
        [JsonProperty("triggerCondition")]
        public Dictionary<string, int> TriggerConditions { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Dictionary of indicator lists for symbols
        /// </summary>
        public Dictionary<string, IEnumerable<BinanceIndicator>> Indicators { get; set; } = new Dictionary<string, IEnumerable<BinanceIndicator>>();

        /// <summary>
        /// Last update time
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// Indicator info
    /// </summary>
    public class BinanceIndicator
    {
        /// <summary>
        /// Indicator name
        /// </summary>
        [JsonProperty("i")]
        public IndicatorType IndicatorType { get; set; }

        /// <summary>
        /// Count
        /// </summary>
        [JsonProperty("c")]
        public int Count { get; set; }

        /// <summary>
        /// Current value
        /// </summary>
        [JsonProperty("v")]
        public decimal CurrentValue { get; set; }

        /// <summary>
        /// Trigger value
        /// </summary>
        [JsonProperty("t")]
        public decimal TriggerValue { get; set; }
    }
}
