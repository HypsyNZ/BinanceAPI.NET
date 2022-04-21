using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class LendingTypeConverter : BaseConverter<LendingType>
    {
        public LendingTypeConverter() : this(true)
        {
        }

        public LendingTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<LendingType, string>> Mapping => new()
        {
            new KeyValuePair<LendingType, string>(LendingType.Activity, "ACTIVITY"),
            new KeyValuePair<LendingType, string>(LendingType.CustomizedFixed, "CUSTOMIZED_FIXED"),
            new KeyValuePair<LendingType, string>(LendingType.Daily, "DAILY")
        };
    }
}