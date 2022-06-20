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

using BinanceAPI.Requests;
using System;

#pragma warning disable CS1591 // WIP

namespace BinanceAPI
{
    public class UriHelper
    {
        public UriHelper(string baseAddress)
        {
            General = new CommonEndpoints(baseAddress);
            Order = new OrderEndpoints(baseAddress);
        }

        public CommonEndpoints General;
        public OrderEndpoints Order;
    }

    public class CommonEndpoints
    {
        public CommonEndpoints(string baseAddress)
        {
            API_V3_TIME_SyncTime = GetUri.New(baseAddress, "time", "api", "3");
        }

        /// <summary>
        /// https://binance-docs.github.io/apidocs/spot/en/#check-server-time
        /// </summary>
        public Uri API_V3_TIME_SyncTime;

    }

    public class OrderEndpoints
    {
        public OrderEndpoints(string baseAddress)
        {
            API_V3_GET_ORDERS_GetOrders = GetUri.New(baseAddress, "order", "api", "3");
        }

        /// <summary>
        /// https://binance-docs.github.io/apidocs/spot/en/#query-order-user_data
        /// </summary>
        public Uri API_V3_GET_ORDERS_GetOrders;
    }

    public class UriManager
    {
        /// <summary>
        /// API1
        /// </summary>
        public static UriHelper API_ONE = new UriHelper("http://api1.binance.com/");

        /////<summary>
        ///// API2
        ///// </summary>
        //public static UriHelper API_TWO = new UriHelper("http://api2.binance.com/");

        ///// <summary>
        ///// API3
        ///// </summary>
        //public static UriHelper API_THREE = new UriHelper("http://api3.binance.com/");
    }
}
