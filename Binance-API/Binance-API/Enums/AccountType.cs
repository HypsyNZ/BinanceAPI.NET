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

using BinanceAPI.Attributes;

namespace BinanceAPI.Enums
{
    /// <summary>
    /// Type of account
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// Spot account type
        /// </summary>
        [Map("SPOT")]
        Spot,
        /// <summary>
        /// Margin account type
        /// </summary>>
        [Map("MARGIN")]
        Margin,
        /// <summary>
        /// Futures account type
        /// </summary>
        [Map("FUTURES")]
        Futures,
        /// <summary>
        /// Leveraged account type
        /// </summary>
        [Map("LEVERAGED")]
        Leveraged,
        /// <summary>
        /// See https://github.com/binance/binance-spot-api-docs/blob/master/rest-api.md#enum-definitions
        /// </summary>
        [Map("TRD_GRP_002")]
        TradeGroup002,
        /// <summary>
        /// See https://github.com/binance/binance-spot-api-docs/blob/master/rest-api.md#enum-definitions
        /// </summary>
        [Map("TRD_GRP_003")]
        TradeGroup003,
        /// <summary>
        /// See https://github.com/binance/binance-spot-api-docs/blob/master/rest-api.md#enum-definitions
        /// </summary>
        [Map("TRD_GRP_004")]
        TradeGroup004
    }
}
