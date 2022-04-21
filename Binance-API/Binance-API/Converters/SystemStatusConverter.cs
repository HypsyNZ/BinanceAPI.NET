using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class SystemStatusConverter : BaseConverter<SystemStatus>
    {
        public SystemStatusConverter() : this(true)
        {
        }

        public SystemStatusConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<SystemStatus, string>> Mapping => new()
        {
            new KeyValuePair<SystemStatus, string>(SystemStatus.Normal, "0"),
            new KeyValuePair<SystemStatus, string>(SystemStatus.Maintenance, "1")
        };
    }
}