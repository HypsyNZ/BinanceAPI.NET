using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class SideEffectTypeConverter : BaseConverter<SideEffectType>
    {
        public SideEffectTypeConverter() : this(true)
        {
        }

        public SideEffectTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<SideEffectType, string>> Mapping => new()
        {
            new KeyValuePair<SideEffectType, string>(SideEffectType.NoSideEffect, "NO_SIDE_EFFECT"),
            new KeyValuePair<SideEffectType, string>(SideEffectType.MarginBuy, "MARGIN_BUY"),
            new KeyValuePair<SideEffectType, string>(SideEffectType.AutoRepay, "AUTO_REPAY"),
        };
    }
}