using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class IsolatedMarginTransferDirectionConverter : BaseConverter<IsolatedMarginTransferDirection>
    {
        public IsolatedMarginTransferDirectionConverter() : this(true)
        {
        }

        public IsolatedMarginTransferDirectionConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<IsolatedMarginTransferDirection, string>> Mapping => new()
        {
            new KeyValuePair<IsolatedMarginTransferDirection, string>(IsolatedMarginTransferDirection.Spot, "SPOT"),
            new KeyValuePair<IsolatedMarginTransferDirection, string>(IsolatedMarginTransferDirection.IsolatedMargin, "ISOLATED_MARGIN"),
        };
    }
}