using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class ListStatusTypeConverter : BaseConverter<ListStatusType>
    {
        public ListStatusTypeConverter() : this(true)
        {
        }

        public ListStatusTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<ListStatusType, string>> Mapping => new()
        {
            new KeyValuePair<ListStatusType, string>(ListStatusType.Response, "RESPONSE"),
            new KeyValuePair<ListStatusType, string>(ListStatusType.ExecutionStarted, "EXEC_STARTED"),
            new KeyValuePair<ListStatusType, string>(ListStatusType.Done, "ALL_DONE"),
        };
    }
}