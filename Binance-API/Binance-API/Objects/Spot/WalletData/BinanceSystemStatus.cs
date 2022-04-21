using BinanceAPI.Converters;
using BinanceAPI.Enums;
using Newtonsoft.Json;

namespace BinanceAPI.Objects.Spot.WalletData
{
    /// <summary>
    /// The status of Binance
    /// </summary>
    public class BinanceSystemStatus
    {
        /// <summary>
        /// Status
        /// </summary>
        [JsonConverter(typeof(SystemStatusConverter))]
        public SystemStatus Status { get; set; }

        /// <summary>
        /// Additional info
        /// </summary>
        [JsonProperty("msg")]
        public string? Message { get; set; }
    }
}