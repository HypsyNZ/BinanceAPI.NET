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
using BinanceAPI.Clients;
using BinanceAPI.Enums;
using BinanceAPI.Objects;
using BinanceAPI.Sockets;
using SimpleLog4.NET;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// 6.0.4.1 Test - Connection Status - https://i.imgur.com/ceKg211.png

namespace API_Test
{
    internal class Program
    {
        private const string socketLogs = @"C:\logSocket.txt";
        private const string clientLogs = @"C:\logClient.txt";
        private const string timeLogs = @"C:\timeClient.txt";

        private static CancellationTokenSource serverTimeStartWaitToken = new CancellationTokenSource();

        private static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.WindowWidth = (int)(Console.LargestWindowWidth / 2.5);
            Console.WindowHeight = (int)(Console.LargestWindowHeight / 2.5);

            // Default Client Options
            var options = new BinanceClientHostOptions()
            {
                BaseAddress = "http://api3.binance.com",
                LogLevel = LogLevel.Trace,
                // LogPath = clientLogs,
                //TimeLogPath = timeLogs,
                LogToConsole = true,
                SyncUpdateTime = 15,
                ReceiveWindow = TimeSpan.FromMilliseconds(1000)
            };

            BinanceClientHost.SetDefaultOptions(options);

            // Default Socket Client Options
            SocketClientHost.SetDefaultOptions(new SocketClientHostOptions()
            {
                LogLevel = LogLevel.Debug,
                LogPath = socketLogs,
                LogToConsole = false,
                MaxConcurrentResubscriptionsPerSocket = 5,
                MaxReconnectTries = 50
            });

            var YouDontNeedToCopyTheDefaultsBtw = new BinanceClientHostOptions();
            var TheDefaultsThatYouSetOnLineSeventy = BinanceClientHost.DefaultOptions;

            _ = true; // breakpoint here and read the values;

            //#if DEBUG
            //            Json.ShouldCheckObjects = true;
            //            Json.OutputOriginalData = true;
            //#endif

            // API Keys need to be set to go any further
            // They don't have to be valid they just have to be set
            BaseClient.SetAuthentication("ReplaceWithYourKeysBeforeTest", "ReplaceWithYourKeysBeforeTest");

            // Create a Binance Client, It will start the Server Time Client for you
            BinanceClientHost client = new BinanceClientHost(serverTimeStartWaitToken.Token);
            SocketClientHost socketClient = new SocketClientHost();

            // [Version 6.0.3.0] Start Test
            Trace.WriteLine("Starting Test..");
            Task.Run(async () =>
            {
                // Authenticated requests will fail until the ServerTimeClient is Ready
                if (!ServerTimeClient.IsReady())
                {
                    // ServerTimeClient.WaitForStart()
                    Console.WriteLine("Server Time Client Started in " + await ServerTimeClient.WaitForStart(serverTimeStartWaitToken.Token).ConfigureAwait(false) + "ms");
                }

                // Stop the Time Client
                ServerTimeClient.Stop();
                Console.WriteLine("False: " + ServerTimeClient.Exists);

                // Start The Time Client (Which will automatically wait for it to start)
                await ServerTimeClient.Start(options, serverTimeStartWaitToken.Token);
                Console.WriteLine("True: " + ServerTimeClient.Exists);

                // Ping Server
                var r = client.Spot.System.PingAsync().Result;
                if (!r.Success)
                {
                    Console.WriteLine("Error");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Server Ping Ticks: " + r.Data.ToString());
                }

                // Get Account Status
                var status = await client.General.GetAccountStatusAsync();
                if (status.Success)
                {
                    Console.WriteLine("Account Status: " + status.Data.Data.ToString());
                }
                else
                {
                    Console.WriteLine("Error: " + status.Error.ToString());
                }

                // EXCHANGE INFO
                _ = Task.Run(async () =>
                {
                    var result = await client.Spot.System.GetExchangeInfoAsync().ConfigureAwait(false);

                    if (result.Success)
                    {
                        Console.WriteLine("Passed loaded exchange info for: [" + result.Data.Symbols.Count() + "] symbols");
                    }
                }).ConfigureAwait(false);

                //// ORDER TEST
                //_ = Task.Run(async () =>
                //{
                //    DateTime testStartTime = DateTime.UtcNow + TimeSpan.FromMinutes(10);
                //    Stopwatch testTime = Stopwatch.StartNew();
                //    while (true)
                //    {
                //        if (testStartTime < DateTime.UtcNow)
                //        {
                //            Console.WriteLine("Passed");
                //            return;
                //        }

                //        await Task.Delay(200).ConfigureAwait(false);

                //        // Guesser Test
                //        var serverTime = ServerTimeClient.GetServerTimeTicksAsync().Result;
                //        var guess = ServerTimeClient.GetRequestTimestampLong();
                //        var guessAheadBy = (guess - serverTime) / 10000;
                //        // Guesser Test

                //        Console.WriteLine("Guess: " + guess.ToString() + " | ServerTime: " + serverTime.ToString() + "| GuessAheadBy: " + guessAheadBy);

                //        if (guessAheadBy < 1000 && guessAheadBy > -1000)
                //        {
                //            var result = await client.Spot.Order.PlaceTestOrderAsync("TestOrder", BinanceAPI.Enums.OrderSide.Sell, BinanceAPI.Enums.OrderType.Market, quantity: 123).ConfigureAwait(false);
                //            if (!r.Success)
                //            {
                //                Console.WriteLine("Order Failed: " + r.Error.ToString());
                //                return;
                //            }
                //            else
                //            {
                //                Console.WriteLine(r.Success.ToString() + "| MissedPings: " + ServerTimeClient.MissedPingCount + "| CorrectionAttempts: " + ServerTimeClient.CorrectionCount + "| GuesserRanToCompletion: " + ServerTimeClient.GuesserAttemptCount);
                //            }
                //        }
                //        else
                //        {
                //            Console.WriteLine("Ahead/Behind Server Time by 1000ms");
                //            return;
                //        }
                //    }
                //}).ConfigureAwait(false);

                // SOCKET TEST
                _ = Task.Run(() =>
                {
                    //var _breakpoint = socketClient;
                    //_ = _breakpoint;

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(2000).ConfigureAwait(false);

                        UpdateSubscription sub = socketClient.Spot.SubscribeToBookTickerUpdatesAsync("BTCUSDT", data =>
                        {
                            // Uncomment to see output from the Socket
                            Console.WriteLine("[" + data.Data.UpdateId + "] | BestAsk: " + data.Data.BestAskPrice.Normalize().ToString("0.00000") + "| BestBid :" + data.Data.BestBidPrice.Normalize().ToString("0.00000"));
                        }).Result.Data;

                        sub.StatusChanged += BinanceSocket_StatusChanged;

                        await Task.Delay(2000).ConfigureAwait(false);

                        // Reconnect Update Subscription
                        await sub.ReconnectAsync().ConfigureAwait(false);

                        await Task.Delay(5000).ConfigureAwait(false);

                        // Reconnect Update Subscription
                        await sub.ReconnectAsync().ConfigureAwait(false);

                        //// TEST BEGINS
                        //for (int i = 0; i < 1000; i++)
                        //{
                        //    await Task.Delay(1).ConfigureAwait(false);

                        //    // Last Subscription Socket Action Time In Ticks
                        //    Console.WriteLine(sub.Connection.Socket.LastActionTime.Ticks);
                        //}

                        // _ = socketClient.UnsubscribeAllAsync();
                    }).ConfigureAwait(false);
                });
            });

            Console.ReadLine();
        }

        private static void BinanceSocket_StatusChanged(ConnectionStatus obj)
        {
            Console.WriteLine(obj.ToString());
        }
    }
}
