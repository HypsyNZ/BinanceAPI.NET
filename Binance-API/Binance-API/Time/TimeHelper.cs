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

using System;

namespace BinanceAPI.Time
{
    /// <summary>
    /// Helpers for the Time Client
    /// </summary>
    public class TimeHelper
    {
        internal const string SYNC_ERROR = "Failed to Synchronize the time at: ";
        private const string TRIM_ERROR = "TRIM would remove all PING_VALUES, Increase PING_VALUES or Reduce TRIM";

        internal const int TICKS_PER_MILLISECOND = 10000;
        private static readonly long EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        /// <summary>
        /// Create a UNIX timestamp from Epoch Time
        /// </summary>
        /// <param name="timeAsTicks"></param>
        /// <returns></returns>
        public static long ToUnixTimestamp(long timeAsTicks)
        {
            return (timeAsTicks - EpochTime) / TICKS_PER_MILLISECOND;
        }
    }
}
