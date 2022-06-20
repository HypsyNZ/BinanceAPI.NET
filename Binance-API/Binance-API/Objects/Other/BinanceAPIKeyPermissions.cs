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

using BinanceAPI.Converters;
using Newtonsoft.Json;
using System;

namespace BinanceAPI.Objects.Other
{
    /// <summary>
    /// Permissions of the current API key
    /// </summary>
    public class BinanceAPIKeyPermissions
    {
        /// <summary>
        /// Whether the key is restricted to certain IP's or not
        /// </summary>
        public bool IpRestrict { get; set; }

        /// <summary>
        /// Creation time of the key
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// This option allows you to withdraw via API. You must apply the IP Access Restriction filter in order to enable withdrawals
        /// </summary>
        public bool EnableWithdrawals { get; set; }

        /// <summary>
        /// This option authorizes this key to transfer funds between your master account and your sub account instantly
        /// </summary>
        public bool PermitUniversalTransfer { get; set; }

        /// <summary>
        /// Authorizes this key to be used for a dedicated universal transfer API to transfer multiple supported currencies. Each business's own transfer API rights are not affected by this authorization
        /// </summary>
        public bool EnableInternalTransfer { get; set; }

        /// <summary>
        /// Authorizes this key to Vanilla options trading
        /// </summary>
        public bool EnableVanillaOptions { get; set; }

        /// <summary>
        /// Authorizes the reading of account info
        /// </summary>
        public bool EnableReading { get; set; }

        /// <summary>
        /// Authorizes futures trading. API Key created before your futures account opened does not support futures API service
        /// </summary>
        public bool EnableFutures { get; set; }

        /// <summary>
        /// Authorizes margin. This option can be adjusted after the Cross Margin account transfer is completed
        /// </summary>
        public bool EnableMargin { get; set; }

        /// <summary>
        /// Spot and margin trading allowed
        /// </summary>
        public bool EnableSpotAndMarginTrading { get; set; }

        /// <summary>
        /// Expiration time for spot and margin trading permission
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime? TradingAuthorityExpirationTime { get; set; }
    }
}
