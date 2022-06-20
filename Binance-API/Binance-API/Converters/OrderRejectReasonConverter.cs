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
    internal class OrderRejectReasonConverter : BaseConverter<OrderRejectReason>
    {
        public OrderRejectReasonConverter() : this(true)
        {
        }

        public OrderRejectReasonConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<OrderRejectReason, string>> Mapping => new()
        {
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.None, "NONE"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.UnknownInstrument, "UNKNOWN_INSTRUMENT"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.MarketClosed, "MARKET_CLOSED"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.PriceQuantityExceedsHardLimits, "PRICE_QTY_EXCEED_HARD_LIMITS"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.UnknownOrder, "UNKNOWN_ORDER"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.DuplicateOrder, "DUPLICATE_ORDER"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.UnknownAccount, "UNKNOWN_ACCOUNT"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.InsufficientBalance, "INSUFFICIENT_BALANCE"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.AccountInactive, "ACCOUNT_INACTIVE"),
            new KeyValuePair<OrderRejectReason, string>(OrderRejectReason.AccountCannotSettle, "ACCOUNT_CANNOT_SETTLE")
        };
    }
}
