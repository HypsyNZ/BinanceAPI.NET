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
    /// Universal transfer type
    /// </summary>
    public enum UniversalTransferType
    {
        /// <summary>
        /// Main (spot) to Funding
        /// </summary>
        MainToFunding = 0,

        /// <summary>
        /// Main (spot) to Usd Futures
        /// </summary>
        MainToUsdFutures = 1,

        /// <summary>
        /// Main (spot) to Coin Futures
        /// </summary>
        MainToCoinFutures = 2,

        /// <summary>
        /// Main (spot) to Margin
        /// </summary>
        MainToMargin = 3,

        /// <summary>
        /// Main (spot) to Mining
        /// </summary>
        MainToMining = 4,

        /// <summary>
        /// Funding to Main (spot)
        /// </summary>
        FundingToMain = 5,

        /// <summary>
        /// Funding to Usd futures
        /// </summary>
        FundingToUsdFutures = 6,

        /// <summary>
        /// Funding to margin
        /// </summary>
        FundingToMargin = 7,

        /// <summary>
        /// Usd futures to Main (spot)
        /// </summary>
        UsdFuturesToMain = 8,

        /// <summary>
        /// Usd futures to Funding
        /// </summary>
        UsdFuturesToFunding = 9,

        /// <summary>
        /// Usd futures to Margin
        /// </summary>
        UsdFuturesToMargin = 10,

        /// <summary>
        /// Coin futures to Main (spot)
        /// </summary>
        CoinFuturesToMain = 11,

        /// <summary>
        /// Coin futures to Margin
        /// </summary>
        CoinFuturesToMargin = 12,

        /// <summary>
        /// Margin to Main (spot)
        /// </summary>
        MarginToMain = 13,

        /// <summary>
        /// Margin to Usd futures
        /// </summary>
        MarginToUsdFutures = 14,

        /// <summary>
        /// Margin to Coin futures
        /// </summary>
        MarginToCoinFutures = 15,

        /// <summary>
        /// Margin to Mining
        /// </summary>
        MarginToMining = 16,

        /// <summary>
        /// Margin to Funding
        /// </summary>
        MarginToFunding = 17,

        /// <summary>
        /// Isolated margin to margin
        /// </summary>
        IsolatedMarginToMargin = 18,

        /// <summary>
        /// Margin to isolated margin
        /// </summary>
        MarginToIsolatedMargin = 19,

        /// <summary>
        /// Isolated margin to Isolated margin
        /// </summary>
        IsolatedMarginToIsolatedMargin = 20,

        /// <summary>
        /// Mining to Main (spot)
        /// </summary>
        MiningToMain = 21,

        /// <summary>
        /// Mining to Usd futures
        /// </summary>
        MiningToUsdFutures = 22,

        /// <summary>
        /// Mining to Margin
        /// </summary>
        MiningToMargin = 23,

        /// <summary>
        /// Funding to Coin futures
        /// </summary>
        FundingToCoinFutures = 24,

        /// <summary>
        /// Coin futures to Funding
        /// </summary>
        CoinFuturesToFunding = 25,
    }
}
