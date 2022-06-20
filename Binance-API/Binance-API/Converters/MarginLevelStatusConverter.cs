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
    internal class MarginLevelStatusConverter : BaseConverter<MarginLevelStatus>
    {
        public MarginLevelStatusConverter() : this(true)
        {
        }

        public MarginLevelStatusConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<MarginLevelStatus, string>> Mapping => new()
        {
            new KeyValuePair<MarginLevelStatus, string>(MarginLevelStatus.Excessive, "EXCESSIVE"),
            new KeyValuePair<MarginLevelStatus, string>(MarginLevelStatus.Normal, "NORMAL"),
            new KeyValuePair<MarginLevelStatus, string>(MarginLevelStatus.MarginCall, "MARGIN_CALL"),
            new KeyValuePair<MarginLevelStatus, string>(MarginLevelStatus.PreLiquidation, "PRE_LIQUIDATION"),
            new KeyValuePair<MarginLevelStatus, string>(MarginLevelStatus.ForceLiquidation, "FORCE_LIQUIDATION")
        };
    }
}
