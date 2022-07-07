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

using BinanceAPI.ClientBase;
using BinanceAPI.UriBase;
using System;

namespace BinanceAPI
{
    /// <summary>
    /// The Static Client that manages Uris for Requests
    /// </summary>
    public class UriClient
    {
        private static readonly UriHelper _allEndpoints;

        private const string API_CONTROLLER_DEFAULT = @"https://api.binance.com/";
        private const string API_CONTROLLER_ONE = @"https://api1.binance.com/";
        private const string API_CONTROLLER_TWO = @"https://api2.binance.com/";
        private const string API_CONTROLLER_THREE = @"https://api3.binance.com/";
        private const string API_CONTROLLER_TEST = @"https://testnet.binance.vision/";

        private static readonly string API_DEFAULT;
        private static readonly string API_ONE;
        private static readonly string API_TWO;
        private static readonly string API_THREE;
        private static readonly string API_TEST;

        private const string WSS_STREAM_DEFAULT = @"wss://stream.binance.com:9443/stream";
        private const string WSS_STREAM_ONE = @"wss://stream1.binance.com:9443/stream";
        private const string WSS_STREAM_TWO = @"wss://stream2.binance.com:9443/stream";
        private const string WSS_STREAM_THREE = @"wss://stream3.binance.com:9443/stream";
        private const string WSS_STREAM_TEST = @"wss://testnet.binance.vision/stream";

        private static readonly Uri WSS_DEFAULT;
        private static readonly Uri WSS_ONE;
        private static readonly Uri WSS_TWO;
        private static readonly Uri WSS_THREE;
        private static readonly Uri WSS_TEST;

        static UriClient()
        {
            _allEndpoints = new UriHelper();

            WSS_DEFAULT = new Uri(WSS_STREAM_DEFAULT);
            WSS_ONE = new Uri(WSS_STREAM_ONE);
            WSS_TWO = new Uri(WSS_STREAM_TWO);
            WSS_THREE = new Uri(WSS_STREAM_THREE);
            WSS_TEST = new Uri(WSS_STREAM_TEST);

            API_DEFAULT = API_CONTROLLER_DEFAULT;
            API_ONE = API_CONTROLLER_ONE;
            API_TWO = API_CONTROLLER_TWO;
            API_THREE = API_CONTROLLER_THREE;
            API_TEST = API_CONTROLLER_TEST;
        }

        /// <summary>
        /// API Endpoint Uri Helper
        /// </summary>
        public static UriHelper GetEndpoint => _allEndpoints;

        /// <summary>
        /// Get the current Api Endpoint Base Address
        /// </summary>
        /// <returns></returns>
        public static string GetBaseAddress()
        {
            switch (BaseClient.ChangeEndpoint)
            {
                case ApiEndpoint.DEFAULT:
                    return API_DEFAULT;
                case ApiEndpoint.ONE:
                    return API_ONE;
                case ApiEndpoint.TWO:
                    return API_TWO;
                case ApiEndpoint.THREE:
                    return API_THREE;
                case ApiEndpoint.TEST:
                    return API_TEST;
                default:
                    return API_DEFAULT;
            }
        }

        /// <summary>
        /// Get the current Web Socket Stream for the Endpoint Controller that is currently selected
        /// </summary>
        /// <returns></returns>
        public static Uri GetStream()
        {
            switch (BaseClient.ChangeEndpoint)
            {
                case ApiEndpoint.DEFAULT:
                    return WSS_DEFAULT;
                case ApiEndpoint.ONE:
                    return WSS_ONE;
                case ApiEndpoint.TWO:
                    return WSS_TWO;
                case ApiEndpoint.THREE:
                    return WSS_THREE;
                case ApiEndpoint.TEST:
                    return WSS_TEST;
                default:
                    return WSS_DEFAULT;
            }
        }
    }
}
