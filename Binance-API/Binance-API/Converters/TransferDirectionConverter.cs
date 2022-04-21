using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class TransferDirectionConverter : BaseConverter<TransferDirection>
    {
        public TransferDirectionConverter() : this(true)
        {
        }

        public TransferDirectionConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<TransferDirection, string>> Mapping => new()
        {
            new KeyValuePair<TransferDirection, string>(TransferDirection.RollIn, "ROLL_IN"),
            new KeyValuePair<TransferDirection, string>(TransferDirection.RollOut, "ROLL_OUT"),
        };
    }
}