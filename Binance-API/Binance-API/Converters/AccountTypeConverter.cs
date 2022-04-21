using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class AccountTypeConverter : BaseConverter<AccountType>
    {
        public AccountTypeConverter() : this(true)
        {
        }

        public AccountTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<AccountType, string>> Mapping => new()
        {
            new KeyValuePair<AccountType, string>(AccountType.Spot, "SPOT"),
            new KeyValuePair<AccountType, string>(AccountType.Margin, "MARGIN"),
            new KeyValuePair<AccountType, string>(AccountType.Futures, "FUTURES"),
            new KeyValuePair<AccountType, string>(AccountType.Leveraged, "LEVERAGED"),
            new KeyValuePair<AccountType, string>(AccountType.TRD_GRP_002, "TRD_GRP_002"),
            new KeyValuePair<AccountType, string>(AccountType.TRD_GRP_003, "TRD_GRP_003")
        };
    }
}