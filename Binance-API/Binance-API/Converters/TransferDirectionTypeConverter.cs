using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class TransferDirectionTypeConverter : BaseConverter<TransferDirectionType>
    {
        public TransferDirectionTypeConverter() : this(true)
        {
        }

        public TransferDirectionTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<TransferDirectionType, string>> Mapping => new()
        {
            new KeyValuePair<TransferDirectionType, string>(TransferDirectionType.MainToMargin, "1"),
            new KeyValuePair<TransferDirectionType, string>(TransferDirectionType.MarginToMain, "2")
        };
    }
}