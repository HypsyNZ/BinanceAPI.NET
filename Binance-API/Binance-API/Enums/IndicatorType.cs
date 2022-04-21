using BinanceAPI.Converters;
using Newtonsoft.Json;

namespace BinanceAPI.Enums
{
    /// <summary>
    /// Types of indicators
    /// </summary>
    [JsonConverter(typeof(IndicatorTypeConverter))]
    public enum IndicatorType
    {
        /// <summary>
        /// Unfilled ratio
        /// </summary>
        UnfilledRatio,

        /// <summary>
        /// Expired orders ratio
        /// </summary>
        ExpirationRatio,

        /// <summary>
        /// Cancelled orders ratio
        /// </summary>
        CancellationRatio
    }
}