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
using BinanceAPI.Objects;
using BinanceAPI.Sockets;
using SimpleLog4.NET;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// 6.0.3.0 Test - Time Sync Changes - https://i.imgur.com/KLI7Jat.png
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
            var options = new BinanceClientOptions()
            {
                BaseAddress = "http://api3.binance.com",
                LogLevel = LogLevel.Trace,
                // LogPath = clientLogs,
                //TimeLogPath = timeLogs,
                LogToConsole = true,
                SyncUpdateTime = 15,
                ReceiveWindow = TimeSpan.FromMilliseconds(1000)
            };

            BinanceClient.SetDefaultOptions(options);

            // Default Socket Client Options
            SocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
            {
                LogLevel = LogLevel.Debug,
                LogPath = socketLogs,
                LogToConsole = false,
                MaxConcurrentResubscriptionsPerSocket = 5,
                MaxReconnectTries = 50
            });

            var YouDontNeedToCopyTheDefaultsBtw = new BinanceClientOptions();
            var TheDefaultsThatYouSetOnLineSeventy = BinanceClient.DefaultOptions;

            //_ = true; // breakpoint here and read the values;

            //#if DEBUG
            //            Json.ShouldCheckObjects = true;
            //            Json.OutputOriginalData = true;
            //#endif

            // API Keys need to be set to go any further
            // They don't have to be valid they just have to be set
            BaseClient.SetAuthentication("ReplaceWithYourKeysBeforeTest", "ReplaceWithYourKeysBeforeTest");

            // Create a Binance Client, It will start the Server Time Client for you
            BinanceClient client = new BinanceClient(serverTimeStartWaitToken.Token);
            SocketClient socketClient = new SocketClient();

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
                //_ = Task.Run(() =>
                //{
                //    // // // // // // // // // // // //

                //    //var _breakpoint = socketClient;
                //    //_ = _breakpoint;

                //    _ = Task.Run(async () =>
                //    {
                //        await Task.Delay(2000).ConfigureAwait(false);

                //        UpdateSubscription sub = socketClient.Spot.SubscribeToBookTickerUpdatesAsync("BTCUSDT", data =>
                //        {
                //            // Uncomment to see output from the Socket
                //            //Console.WriteLine("[" + data.Data.UpdateId + "] | BestAsk: " + data.Data.BestAskPrice.Normalize().ToString("0.00000") + "| BestBid :" + data.Data.BestBidPrice.Normalize().ToString("0.00000"));
                //        }).Result.Data;

                //        await Task.Delay(2000).ConfigureAwait(false);

                //        // Reconnect Update Subscription
                //        await sub.ReconnectAsync().ConfigureAwait(false);

                //        // TEST BEGINS
                //        for (int i = 0; i < 1000; i++)
                //        {
                //            await Task.Delay(1).ConfigureAwait(false);

                //            // Last Subscription Socket Action Time In Ticks
                //            Console.WriteLine(sub.Connection.Socket.LastActionTime.Ticks);
                //        }

                //        _ = socketClient.UnsubscribeAllAsync();
                //    }).ConfigureAwait(false);

                //    // // // // // // // // // // // //
                //});
            });

            Console.ReadLine();
        }

        // Socket Log
        //[06/18/22 01:01:44:589][Debug] Socket 1 new socket created for wss://stream.binance.com:9443/stream
        //[06/18/22 01:01:44:593][Debug] Socket 1 connecting
        //[06/18/22 01:01:45:595][Debug] Socket 1 connected
        //[06/18/22 01:01:45:603][Debug] Socket 1 connected to wss://stream.binance.com:9443/stream with request {"method":"SUBSCRIBE","params":["btcusdt@bookTicker"],"id":9}
        //[06/18/22 01:01:45:604][Debug] Socket 1 sending data: {"method":"SUBSCRIBE","params":["btcusdt@bookTicker"],"id":9}
        //[06/18/22 01:01:47:802][Debug] Socket 1 closing
        //[06/18/22 01:01:47:826][Debug] Socket 1 received `Close` message
        //[06/18/22 01:01:47:826][Debug] Socket 1 closed
        //[06/18/22 01:01:47:828][Info] Socket 1 Connection lost, will try to reconnect after 2000ms
        //[06/18/22 01:01:49:809][Debug] Socket 1 resetting
        //[06/18/22 01:01:49:809][Debug] Socket 1 connecting
        //[06/18/22 01:01:50:555][Debug] Socket 1 connected
        //[06/18/22 01:01:57:499][Info][00:00:09.6722449] Socket[1] Reconnected - Attempts: 1/50
        //[06/18/22 01:01:57:500][Debug] Socket 1 sending data: {"method":"SUBSCRIBE","params":["btcusdt@bookTicker"],"id":9}
        //[06/18/22 01:01:57:719][Debug] Socket 1 subscription successfully resubscribed on reconnected socket.
        //[06/18/22 01:01:57:719][Debug] Socket 1 data connection restored.
    }
}
