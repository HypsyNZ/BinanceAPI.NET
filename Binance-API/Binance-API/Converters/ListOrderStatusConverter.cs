using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class ListOrderStatusConverter : BaseConverter<ListOrderStatus>
    {
        public ListOrderStatusConverter() : this(true)
        {
        }

        public ListOrderStatusConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<ListOrderStatus, string>> Mapping => new()
        {
            new KeyValuePair<ListOrderStatus, string>(ListOrderStatus.Executing, "EXECUTING"),
            new KeyValuePair<ListOrderStatus, string>(ListOrderStatus.Rejected, "REJECT"),
            new KeyValuePair<ListOrderStatus, string>(ListOrderStatus.Done, "ALL_DONE"),
        };
    }
}