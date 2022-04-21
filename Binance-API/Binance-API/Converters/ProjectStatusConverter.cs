using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class ProjectStatusConverter : BaseConverter<ProjectStatus>
    {
        public ProjectStatusConverter() : this(true)
        {
        }

        public ProjectStatusConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<ProjectStatus, string>> Mapping => new()
        {
            new KeyValuePair<ProjectStatus, string>(ProjectStatus.Holding, "HOLDING"),
            new KeyValuePair<ProjectStatus, string>(ProjectStatus.Redeemed, "REDEEMED")
        };
    }
}