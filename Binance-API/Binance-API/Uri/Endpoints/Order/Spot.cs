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

#pragma warning disable CS1591 // WIP

namespace BinanceAPI.UriBase
{
    public class Spot
    {
        public const string NEW_TEST_ORDER = "order/test";
        public const string NEW_OCO_ORDER = "order/oco";

        public const string ORDER = "order";
        public const string OCO_ORDER = "orderList";
        public const string OPEN_ORDERS = "openOrders";
        public const string ALL_ORDERS = "allOrders";

        public const string ALL_TRADES = "myTrades";

        public const string ALL_OCO_ORDERS = "allOrderList";
        public const string ALL_OPEN_OCO_ORDERS = "openOrderList";

        public const string API = "api";
        public const string VERSION = "3";

        public Spot()
        {
            // Order
            POST_NEW_ORDER_NewOrder = GetUriString.Combine(ORDER, API, VERSION);
            POST_NEW_ORDER_TEST_NewOrderTest = GetUriString.Combine(NEW_TEST_ORDER, API, VERSION);
            GET_SINGLE_ORDER_GetSingleOrder = GetUriString.Combine(ORDER, API, VERSION);
            GET_OPEN_ORDERS_GetAllOpenOrders = GetUriString.Combine(OPEN_ORDERS, API, VERSION);
            GET_ALL_ORDERS_GetAllOrders = GetUriString.Combine(ALL_ORDERS, API, VERSION);
            DELETE_CANCEL_ORDER_CancelOrder = GetUriString.Combine(ORDER, API, VERSION);
            DELETE_CANCEL_ALL_ORDERS_CancelAllOpenOrders = GetUriString.Combine(OPEN_ORDERS, API, VERSION);

            // Trades
            GET_ALL_TRADES_GetAllTrades = GetUriString.Combine(ALL_TRADES, API, VERSION);

            // Oco Order
            POST_NEW_ORDER_OCO_NewOcoOrder = GetUriString.Combine(NEW_OCO_ORDER, API, VERSION);
            GET_ORDER_OCO_GetOcoOrder = GetUriString.Combine(OCO_ORDER, API, VERSION);
            GET_ALL_ORDER_OCO_GetAllOcoOrders = GetUriString.Combine(ALL_OCO_ORDERS, API, VERSION);
            GET_ALL_OPEN_OCO_ORDERS_GetAllOpenOcoOrders = GetUriString.Combine(ALL_OPEN_OCO_ORDERS, API, VERSION);
            DELETE_CANCEL_ORDER_OCO_CancelOcoOrder = GetUriString.Combine(OCO_ORDER, API, VERSION);
        }

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#new-order-trade
        /// </summary>
        public readonly string POST_NEW_ORDER_NewOrder;

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#new-oco-trade
        /// </summary>
        public readonly string POST_NEW_ORDER_OCO_NewOcoOrder;

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#test-new-order-trade
        /// </summary>
        public readonly string POST_NEW_ORDER_TEST_NewOrderTest;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-order-user_data
        /// </summary>
        public readonly string GET_SINGLE_ORDER_GetSingleOrder;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#current-open-orders-user_data
        /// </summary>
        public readonly string GET_OPEN_ORDERS_GetAllOpenOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#all-orders-user_data
        /// </summary>
        public readonly string GET_ALL_ORDERS_GetAllOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#account-trade-list-user_data
        /// </summary>
        public readonly string GET_ALL_TRADES_GetAllTrades;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-oco-user_data
        /// </summary>
        public readonly string GET_ORDER_OCO_GetOcoOrder;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-all-oco-user_data
        /// </summary>
        public readonly string GET_ALL_ORDER_OCO_GetAllOcoOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-open-oco-user_data
        /// </summary>
        public readonly string GET_ALL_OPEN_OCO_ORDERS_GetAllOpenOcoOrders;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#cancel-order-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ORDER_CancelOrder;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#cancel-oco-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ORDER_OCO_CancelOcoOrder;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#cancel-all-open-orders-on-a-symbol-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ALL_ORDERS_CancelAllOpenOrders;
    }
}
