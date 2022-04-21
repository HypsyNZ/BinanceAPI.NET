using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class IndicatorTypeConverter : BaseConverter<IndicatorType>
    {
        public IndicatorTypeConverter() : this(true)
        {
        }

        public IndicatorTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<IndicatorType, string>> Mapping => new()
        {
            new KeyValuePair<IndicatorType, string>(IndicatorType.CancellationRatio, "GCR"),
            new KeyValuePair<IndicatorType, string>(IndicatorType.UnfilledRatio, "UFR"),
            new KeyValuePair<IndicatorType, string>(IndicatorType.ExpirationRatio, "IFER")
        };
    }
}