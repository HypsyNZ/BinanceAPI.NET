using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class ProjectTypeConverter : BaseConverter<ProjectType>
    {
        public ProjectTypeConverter() : this(true)
        {
        }

        public ProjectTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<ProjectType, string>> Mapping => new()
        {
            new KeyValuePair<ProjectType, string>(ProjectType.CustomizedFixed, "CUSTOMIZED_FIXED"),
            new KeyValuePair<ProjectType, string>(ProjectType.Activity, "ACTIVITY")
        };
    }
}