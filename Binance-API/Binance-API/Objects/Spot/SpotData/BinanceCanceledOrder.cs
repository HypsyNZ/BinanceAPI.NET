using BinanceAPI.Objects.Shared;
using System.Globalization;

namespace BinanceAPI.Objects.Spot.SpotData
{
    /// <summary>
    /// Information about a canceled order
    /// </summary>
    public class BinanceCanceledOrder : BinanceOrderBase
    {
        /// <summary>
        /// ID of the Order that is being cancelled
        /// </summary>
        public new string OrderId => OrderId.ToString(CultureInfo.InvariantCulture);
    }
}