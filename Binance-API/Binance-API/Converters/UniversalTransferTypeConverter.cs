/*
*MIT License
*
*Copyright (c) 2022 S Christison
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/

using BinanceAPI.Enums;
using System.Collections.Generic;

namespace BinanceAPI.Converters
{
    internal class UniversalTransferTypeConverter : BaseConverter<UniversalTransferType>
    {
        public UniversalTransferTypeConverter() : this(true)
        {
        }

        public UniversalTransferTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<UniversalTransferType, string>> Mapping => new()
        {
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToFunding, "MAIN_FUNDING"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToUsdFutures, "MAIN_UMFUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToCoinFutures, "MAIN_CMFUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToMargin, "MAIN_MARGIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToMining, "MAIN_MINING"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FundingToMain, "FUNDING_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FundingToUsdFutures, "FUNDING_UMFUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FundingToMargin, "FUNDING_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.UsdFuturesToMain, "UMFUTURE_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.UsdFuturesToFunding, "UMFUTURE_FUNDING"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.UsdFuturesToMargin, "UMFUTURE_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.CoinFuturesToMain, "CMFUTURE_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.CoinFuturesToMargin, "CMFUTURE_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToIsolatedMargin, "MARGIN_ISOLATEDMARGIN "),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.IsolatedMarginToMargin, "ISOLATEDMARGIN_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToMain, "MARGIN_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToUsdFutures, "MARGIN_UMFUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToCoinFutures, "MARGIN_CMFUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToMining, "MARGIN_MINING"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToFunding, "MARGIN_FUNDING"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MiningToMain, "MINING_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MiningToUsdFutures, "MINING_UMFUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MiningToMargin, "MINING_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FundingToCoinFutures, "FUNDING_CMFUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.CoinFuturesToFunding, "CMFUTURE_FUNDING"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.IsolatedMarginToIsolatedMargin, "ISOLATEDMARGIN_ISOLATEDMARGIN"),
        };
    }
}
