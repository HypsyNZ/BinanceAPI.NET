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
using BinanceAPI.Enums;

using Newtonsoft.Json;
using System;

namespace BinanceAPI.Objects.Fiat
{
    /// <summary>
    /// Fiat payment info
    /// </summary>
    public class BinanceFiatPayment
    {
        /// <summary>
        /// Order number
        /// </summary>
        [JsonProperty("orderNo")]
        public string OrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// The input amount
        /// </summary>
        public decimal SourceAmount { get; set; }

        /// <summary>
        /// The fiat currency
        /// </summary>
        public string FiatCurrency { get; set; } = string.Empty;

        /// <summary>
        /// The output amount
        /// </summary>
        public decimal ObtainAmount { get; set; }

        /// <summary>
        /// The crypto currency
        /// </summary>
        public string CryptoCurrency { get; set; } = string.Empty;

        /// <summary>
        /// The total fee of the order
        /// </summary>
        public decimal TotalFee { get; set; }

        /// <summary>
        /// The price of the order
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The status of the order
        /// </summary>
        [JsonConverter(typeof(FiatPaymentStatusConverter))]
        public FiatPaymentStatus Status { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Last update time
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        public DateTime UpdateTime { get; set; }
    }
}
