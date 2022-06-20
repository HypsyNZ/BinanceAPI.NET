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
using BinanceAPI.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// Stream order book
    /// </summary>
    public class BinanceEventOrderBook : BinanceOrderBook
    {
        /// <summary>
        /// The id of this update, can be synced with BinanceClient.Spot.GetOrderBook to update the order book
        /// </summary>
        [JsonProperty("U")]
        public long? FirstUpdateId { get; set; }

        /// <summary>
        /// Setter for last update id, need for Json.Net
        /// </summary>
        [JsonProperty("u")]
        internal long LastUpdateIdStream { set => LastUpdateId = value; }

        /// <summary>
        /// Event type
        /// </summary>
        [JsonProperty("e")]
        internal string EventType { get; set; } = string.Empty;

        /// <summary>
        /// Event time of the update
        /// </summary>
        [JsonProperty("E"), JsonConverter(typeof(TimestampConverter))]
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Setter for bids (needed forJson.Net)
        /// </summary>
        [JsonProperty("b")]
        internal IEnumerable<BinanceOrderBookEntry> BidsStream { set => Bids = value; }

        /// <summary>
        /// Setter for asks (needed forJson.Net)
        /// </summary>
        [JsonProperty("a")]
        internal IEnumerable<BinanceOrderBookEntry> AsksStream { set => Asks = value; }
    }
}
