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
    /// Status of a symbol
    /// </summary>
    public enum SymbolStatus
    {
        /// <summary>
        /// Not trading yet
        /// </summary>
        PreTrading = 0,

        /// <summary>
        /// Pending trading
        /// </summary>
        PendingTrading = 1,

        /// <summary>
        /// Trading
        /// </summary>
        Trading = 2,

        /// <summary>
        /// No longer trading
        /// </summary>
        PostTrading = 3,

        /// <summary>
        /// Not trading
        /// </summary>
        EndOfDay = 4,

        /// <summary>
        /// Halted
        /// </summary>
        Halt = 5,

        /// <summary>
        ///
        /// </summary>
        AuctionMatch = 6,

        /// <summary>
        ///
        /// </summary>
        Break = 7,

        /// <summary>
        /// Closed
        /// </summary>
        Close = 8,

        /// <summary>
        /// Pre delivering
        /// </summary>
        PreDelivering = 9,

        /// <summary>
        /// Delivering
        /// </summary>
        Delivering = 10,

        /// <summary>
        /// Pre settle
        /// </summary>
        PreSettle = 11,

        /// <summary>
        /// Settings
        /// </summary>
        Settling = 12
    }
}
