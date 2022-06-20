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

using BinanceAPI.Authentication;
using BinanceAPI.Clients;
using BinanceAPI.Options;
using BinanceAPI.Time;
using System;
using System.Net.Http;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// Options for the binance client
    /// </summary>
    public class BinanceClientOptions : RestClientOptions
    {
        /// <summary>
        /// Path to the Time Log
        /// </summary>
        public string TimeLogPath { get; set; } = "";

        /// <summary>
        /// The default receive window for requests
        /// </summary>
        public TimeSpan ReceiveWindow { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The rate at which the Server Time Should be Synced in Minutes
        /// <para><see cref="ServerTimeClient.LoopToken"/> can be cancelled to stop syncing</para>
        /// </summary>
        public int SyncUpdateTime { get; set; } = 15;

        /// <summary>
        /// Constructor with default endpoints
        /// </summary>
        public BinanceClientOptions() : this(BinanceApiAddresses.Default)
        {
        }

        /// <summary>
        /// Constructor with default endpoints
        /// </summary>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public BinanceClientOptions(HttpClient client) : this(BinanceApiAddresses.Default, client)
        {
        }

        /// <summary>
        /// Constructor with custom endpoints
        /// </summary>
        /// <param name="addresses">The base addresses to use</param>
        public BinanceClientOptions(BinanceApiAddresses addresses) : this(addresses.RestClientAddress, null)
        {
        }

        /// <summary>
        /// Constructor with custom endpoints
        /// </summary>
        /// <param name="addresses">The base addresses to use</param>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public BinanceClientOptions(BinanceApiAddresses addresses, HttpClient client) : this(addresses.RestClientAddress, client)
        {
        }

        /// <summary>
        /// Constructor with custom endpoints
        /// </summary>
        /// <param name="spotBaseAddress">Сustom url for the SPOT API</param>
        public BinanceClientOptions(string spotBaseAddress) : this(spotBaseAddress, null)
        {
        }

        /// <summary>
        /// Constructor with custom endpoints
        /// </summary>
        /// <param name="spotBaseAddress">Сustom url for the SPOT API</param>
        /// <param name="client">HttpClient to use for requests from this client</param>
        public BinanceClientOptions(string spotBaseAddress, HttpClient? client) : base(spotBaseAddress)
        {
            HttpClient = client;
        }
    }
}
