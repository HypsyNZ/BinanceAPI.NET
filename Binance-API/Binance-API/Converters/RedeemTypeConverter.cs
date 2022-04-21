using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class RedeemTypeConverter : BaseConverter<RedeemType>
    {
        public RedeemTypeConverter() : this(true)
        {
        }

        public RedeemTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<RedeemType, string>> Mapping => new()
        {
            new KeyValuePair<RedeemType, string>(RedeemType.Fast, "FAST"),
            new KeyValuePair<RedeemType, string>(RedeemType.Normal, "NORMAL")
        };
    }
}