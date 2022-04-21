using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class OrderResponseTypeConverter : BaseConverter<OrderResponseType>
    {
        public OrderResponseTypeConverter() : this(true)
        {
        }

        public OrderResponseTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<OrderResponseType, string>> Mapping => new()
        {
            new KeyValuePair<OrderResponseType, string>(OrderResponseType.Acknowledge, "ACK"),
            new KeyValuePair<OrderResponseType, string>(OrderResponseType.Result, "RESULT"),
            new KeyValuePair<OrderResponseType, string>(OrderResponseType.Full, "FULL")
        };
    }
}