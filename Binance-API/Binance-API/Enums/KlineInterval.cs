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
    /// The interval for the kline, the int value represents the time in seconds
    /// </summary>
    public enum KlineInterval
    {
        /// <summary>
        /// 1m
        /// </summary>
        OneMinute = 60,

        /// <summary>
        /// 3m
        /// </summary>
        ThreeMinutes = 60 * 3,

        /// <summary>
        /// 5m
        /// </summary>
        FiveMinutes = 60 * 5,

        /// <summary>
        /// 15m
        /// </summary>
        FifteenMinutes = 60 * 15,

        /// <summary>
        /// 30m
        /// </summary>
        ThirtyMinutes = 60 * 30,

        /// <summary>
        /// 1h
        /// </summary>
        OneHour = 60 * 60,

        /// <summary>
        /// 2h
        /// </summary>
        TwoHour = 60 * 60 * 2,

        /// <summary>
        /// 4h
        /// </summary>
        FourHour = 60 * 60 * 4,

        /// <summary>
        /// 6h
        /// </summary>
        SixHour = 60 * 60 * 6,

        /// <summary>
        /// 8h
        /// </summary>
        EightHour = 60 * 60 * 8,

        /// <summary>
        /// 12h
        /// </summary>
        TwelveHour = 60 * 60 * 12,

        /// <summary>
        /// 1d
        /// </summary>
        OneDay = 60 * 60 * 24,

        /// <summary>
        /// 3d
        /// </summary>
        ThreeDay = 60 * 60 * 24 * 3,

        /// <summary>
        /// 1w
        /// </summary>
        OneWeek = 60 * 60 * 24 * 7,

        /// <summary>
        /// 1M
        /// </summary>
        OneMonth = 60 * 60 * 24 * 30
    }
}
