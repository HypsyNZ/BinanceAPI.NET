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
    public class Margin
    {
        public const string NEW_OCO_ORDER = "margin/order/oco";

        public const string ORDER = "margin/order";
        public const string OCO_ORDER = "margin/orderList";
        public const string OPEN_ORDERS = "margin/openOrders";
        public const string ALL_ORDERS = "margin/allOrders";

        public const string ALL_TRADES = "margin/myTrades";

        public const string ALL_OCO_ORDERS = "margin/allOrderList";
        public const string ALL_OPEN_OCO_ORDERS = "margin/openOrderList";

        public const string API = "sapi";
        public const string VERSION = "1";

        public Margin()
        {
            // Order
            POST_NEW_ORDER_NewOrder = GetUriString.Combine(ORDER, API, VERSION);
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
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-new-order-trade
        /// </summary>
        public readonly string POST_NEW_ORDER_NewOrder;

        /// <summary>
        /// [POST] https://binance-docs.github.io/apidocs/spot/en/#margin-account-new-oco-trade
        /// </summary>
        public readonly string POST_NEW_ORDER_OCO_NewOcoOrder;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-order-user_data
        /// </summary>
        public readonly string GET_SINGLE_ORDER_GetSingleOrder;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-open-orders-user_data
        /// </summary>
        public readonly string GET_OPEN_ORDERS_GetAllOpenOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-all-orders-user_data
        /// </summary>
        public readonly string GET_ALL_ORDERS_GetAllOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-trade-list-user_data
        /// </summary>
        public readonly string GET_ALL_TRADES_GetAllTrades;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-oco-user_data
        /// </summary>
        public readonly string GET_ORDER_OCO_GetOcoOrder;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-all-oco-user_data
        /// </summary>
        public readonly string GET_ALL_ORDER_OCO_GetAllOcoOrders;

        /// <summary>
        /// [GET] https://binance-docs.github.io/apidocs/spot/en/#query-margin-account-39-s-open-oco-user_data
        /// </summary>
        public readonly string GET_ALL_OPEN_OCO_ORDERS_GetAllOpenOcoOrders;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-order-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ORDER_CancelOrder;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-oco-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ORDER_OCO_CancelOcoOrder;

        /// <summary>
        /// [DELETE] https://binance-docs.github.io/apidocs/spot/en/#margin-account-cancel-all-open-orders-on-a-symbol-trade
        /// </summary>
        public readonly string DELETE_CANCEL_ALL_ORDERS_CancelAllOpenOrders;
    }
}
