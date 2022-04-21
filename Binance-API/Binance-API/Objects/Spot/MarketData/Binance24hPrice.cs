using BinanceAPI.Interfaces;
using BinanceAPI.Objects.Shared;
using Newtonsoft.Json;

namespace BinanceAPI.Objects.Spot.MarketData
{
    /// <summary>
    /// Price statistics of the last 24 hours
    /// </summary>
    public class Binance24HPrice : Binance24HPriceBase, IBinanceTick
    {
        /// <summary>
        /// The close price 24 hours ago
        /// </summary>
        [JsonProperty("prevClosePrice")]
        public decimal PrevDayClosePrice { get; set; }

        /// <summary>
        /// The best bid price in the order book
        /// </summary>
        public decimal BidPrice { get; set; }

        /// <summary>
        /// The size of the best bid price in the order book
        /// </summary>
        [JsonProperty("bidQty")]
        public decimal BidQuantity { get; set; }

        /// <summary>
        /// The best ask price in the order book
        /// </summary>
        public decimal AskPrice { get; set; }

        /// <summary>
        /// The size of the best ask price in the order book
        /// </summary>
        [JsonProperty("AskQty")]
        public decimal AskQuantity { get; set; }

        /// <inheritdoc />
        [JsonProperty("volume")]
        public override decimal BaseVolume { get; set; }

        /// <inheritdoc />
        [JsonProperty("quoteVolume")]
        public override decimal QuoteVolume { get; set; }
    }
}