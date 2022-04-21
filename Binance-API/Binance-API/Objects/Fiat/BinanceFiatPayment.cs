﻿using BinanceAPI.Converters;
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