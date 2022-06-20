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
    /// The status of an order
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Order is new
        /// </summary>
        New = 0,

        /// <summary>
        /// Order is partly filled, still has quantity left to fill
        /// </summary>
        PartiallyFilled = 1,

        /// <summary>
        /// The order has been filled and completed
        /// </summary>
        Filled = 2,

        /// <summary>
        /// The order has been canceled
        /// </summary>
        Canceled = 3,

        /// <summary>
        /// The order is in the process of being canceled  (currently unused)
        /// </summary>
        PendingCancel = 4,

        /// <summary>
        /// The order has been rejected
        /// </summary>
        Rejected = 5,

        /// <summary>
        /// The order has expired
        /// </summary>
        Expired = 6,

        /// <summary>
        /// Liquidation with Insurance Fund
        /// </summary>
        Insurance = 7,

        /// <summary>
        /// Counterparty Liquidation
        /// </summary>
        Adl = 8
    }
}
