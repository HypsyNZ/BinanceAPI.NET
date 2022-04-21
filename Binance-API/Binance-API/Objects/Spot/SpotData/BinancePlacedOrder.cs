using BinanceAPI.Attributes;
using BinanceAPI.Converters;
using BinanceAPI.Objects.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects.Spot.SpotData
{
    /// <summary>
    /// The result of placing a new order
    /// </summary>
    public class BinancePlacedOrder : BinanceOrderBase
    {
        /// <summary>
        /// The time the order was placed
        /// </summary>
        [JsonOptionalProperty]
        [JsonProperty("transactTime"), JsonConverter(typeof(TimestampConverter))]
        public new DateTime CreateTime { get; set; }

        /// <summary>
        /// Fills for the order
        /// </summary>
        [JsonOptionalProperty]
        public IEnumerable<BinanceOrderTrade>? Fills { get; set; }

        /// <summary>
        /// Only present if a margin trade happened
        /// </summary>
        [JsonOptionalProperty]
        public decimal? MarginBuyBorrowAmount { get; set; }

        /// <summary>
        /// Only present if a margin trade happened
        /// </summary>
        [JsonOptionalProperty]
        public string? MarginBuyBorrowAsset { get; set; }
    }
}