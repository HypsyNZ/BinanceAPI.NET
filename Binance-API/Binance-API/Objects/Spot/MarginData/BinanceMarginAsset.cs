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

using Newtonsoft.Json;

namespace BinanceAPI.Objects.Spot.MarginData
{
    /// <summary>
    /// Margin asset info
    /// </summary>
    public class BinanceMarginAsset
    {
        /// <summary>
        /// Full name of the asset
        /// </summary>
        [JsonProperty("assetFullName")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Short name of the asset
        /// </summary>
        [JsonProperty("assetName")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Is borrowable
        /// </summary>
        public bool IsBorrowable { get; set; }

        /// <summary>
        /// Is mortgageable
        /// </summary>
        public bool IsMortgageable { get; set; }

        /// <summary>
        /// Minimal amount which can be borrowed
        /// </summary>
        [JsonProperty("userMinBorrow")]
        public decimal MinimalBorrowAmount { get; set; }

        /// <summary>
        /// Minimal amount which can be repaid
        /// </summary>
        [JsonProperty("userMinRepay")]
        public decimal MinimalRepayAmount { get; set; }
    }
}
