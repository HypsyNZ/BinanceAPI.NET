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
    internal class SymbolStatusConverter : BaseConverter<SymbolStatus>
    {
        public SymbolStatusConverter() : this(true)
        {
        }

        public SymbolStatusConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<SymbolStatus, string>> Mapping => new()
        {
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.AuctionMatch, "AUCTION_MATCH"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Break, "BREAK"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.EndOfDay, "END_OF_DAY"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Halt, "HALT"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.PostTrading, "POST_TRADING"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.PreTrading, "PRE_TRADING"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.PendingTrading, "PENDING_TRADING"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Trading, "TRADING"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Close, "CLOSE"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.PreDelivering, "PRE_DELIVERING"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Delivering, "DELIVERING"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.PreSettle, "PRE_SETTLE"),
            new KeyValuePair<SymbolStatus, string>(SymbolStatus.Settling, "SETTLING"),
        };
    }
}
