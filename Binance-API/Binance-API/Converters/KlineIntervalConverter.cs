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
    internal class KlineIntervalConverter : BaseConverter<KlineInterval>
    {
        public KlineIntervalConverter() : this(true)
        {
        }

        public KlineIntervalConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<KlineInterval, string>> Mapping => new()
        {
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneMinute, "1m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThreeMinutes, "3m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FiveMinutes, "5m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FifteenMinutes, "15m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThirtyMinutes, "30m"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneHour, "1h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.TwoHour, "2h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.FourHour, "4h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.SixHour, "6h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.EightHour, "8h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.TwelveHour, "12h"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneDay, "1d"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.ThreeDay, "3d"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneWeek, "1w"),
            new KeyValuePair<KlineInterval, string>(KlineInterval.OneMonth, "1M")
        };
    }
}
