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

using BinanceAPI;
using BinanceAPI.ClientHosts;
using BinanceAPI.Enums;
using BinanceAPI.Sockets;
using System;
using System.Threading.Tasks;

namespace API_Test
{
    // SOCKET TEST
    internal class SocketTest
    {
        public static void Run(SocketClientHost socketClient)
        {
            _ = Task.Run(() =>
            {
                //var _breakpoint = socketClient;
                //_ = _breakpoint;

                _ = Task.Run(async () =>
                {
                    await Task.Delay(2000).ConfigureAwait(false);

                    // Create Update Subscription
                    UpdateSubscription sub = socketClient.Spot.SubscribeToBookTickerUpdatesAsync("BTCUSDT", data =>
                    {
                        // Uncomment to see output from the Socket
                        Console.WriteLine("[" + data.Data.UpdateId +
                            "]| BestAsk: " + data.Data.BestAskPrice.Normalize().ToString("0.00") +
                            " | Ask Quan: " + data.Data.BestAskQuantity.Normalize().ToString("000.00000#####") +
                            " | BestBid :" + data.Data.BestBidPrice.Normalize().ToString("0.00") +
                            " | BidQuantity :" + data.Data.BestBidQuantity.Normalize().ToString("000.00000#####"));
                    }).Result.Data;

                    // Subscribe to Update Subscription Status Changed Events before it connects
                    sub.StatusChanged += BinanceSocket_StatusChanged;

                    // Current Status
                    Console.WriteLine(Enum.GetName(typeof(ConnectionStatus), sub.Connection.SocketConnectionStatus));

                    // work work
                    await Task.Delay(5000).ConfigureAwait(false);

                    // Reconnect Update Subscription
                    await sub.ReconnectAsync().ConfigureAwait(false);

                    // work work
                    await Task.Delay(5000).ConfigureAwait(false);

                    // Reconnect Update Subscription
                    await sub.ReconnectAsync().ConfigureAwait(false);

                    // work work
                    await Task.Delay(20000).ConfigureAwait(false);

                    // Destroy everything and unsubscribe, this should cause UnsubscribeAllAsync to do nothing
                    await sub.CloseAndDisposeAsync().ConfigureAwait(false);

                    //// TEST BEGINS
                    //for (int i = 0; i < 1000; i++)
                    //{
                    //    await Task.Delay(1).ConfigureAwait(false);

                    //    // Last Subscription Socket Action Time In Ticks
                    //    Console.WriteLine(sub.Connection.Socket.LastActionTime.Ticks);
                    //}

                    _ = socketClient.UnsubscribeAllAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);
            });
        }

        private static void BinanceSocket_StatusChanged(ConnectionStatus obj)
        {
            Console.WriteLine(obj.ToString());
        }

    }
}
