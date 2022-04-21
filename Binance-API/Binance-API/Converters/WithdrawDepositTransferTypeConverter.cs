using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class WithdrawDepositTransferTypeConverter : BaseConverter<WithdrawDepositTransferType>
    {
        public WithdrawDepositTransferTypeConverter() : this(true)
        {
        }

        public WithdrawDepositTransferTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<WithdrawDepositTransferType, string>> Mapping =>
            new()
            {
                new KeyValuePair<WithdrawDepositTransferType, string>(
                    WithdrawDepositTransferType.Internal, "1"),
                new KeyValuePair<WithdrawDepositTransferType, string>(
                    WithdrawDepositTransferType.External, "0"),
            };
    }
}