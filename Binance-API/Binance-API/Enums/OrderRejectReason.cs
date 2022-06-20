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

namespace BinanceAPI.Enums
{
    /// <summary>
    /// The reason the order was rejected
    /// </summary>
    public enum OrderRejectReason
    {
        /// <summary>
        /// Not rejected
        /// </summary>
        None = 0,

        /// <summary>
        /// Unknown instrument
        /// </summary>
        UnknownInstrument = 1,

        /// <summary>
        /// Closed market
        /// </summary>
        MarketClosed = 2,

        /// <summary>
        /// Quantity out of bounds
        /// </summary>
        PriceQuantityExceedsHardLimits = 3,

        /// <summary>
        /// Unknown order
        /// </summary>
        UnknownOrder = 4,

        /// <summary>
        /// Duplicate
        /// </summary>
        DuplicateOrder = 5,

        /// <summary>
        /// Unkown account
        /// </summary>
        UnknownAccount = 6,

        /// <summary>
        /// Not enough balance
        /// </summary>
        InsufficientBalance = 7,

        /// <summary>
        /// Account not active
        /// </summary>
        AccountInactive = 8,

        /// <summary>
        /// Cannot settle
        /// </summary>
        AccountCannotSettle = 9
    }
}
