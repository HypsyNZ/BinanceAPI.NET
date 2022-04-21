using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class DepositStatusConverter : BaseConverter<DepositStatus>
    {
        public DepositStatusConverter() : this(true)
        {
        }

        public DepositStatusConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<DepositStatus, string>> Mapping => new()
        {
            new KeyValuePair<DepositStatus, string>(DepositStatus.Pending, "0"),
            new KeyValuePair<DepositStatus, string>(DepositStatus.Success, "1"),
            new KeyValuePair<DepositStatus, string>(DepositStatus.Completed, "6"),
        };
    }
}