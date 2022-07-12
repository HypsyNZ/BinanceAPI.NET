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
using BinanceAPI.ClientBase;
using BinanceAPI.ClientHosts;
using BinanceAPI.Objects;
using SimpleLog4.NET;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

// 6.0.6.1 Test - [AllowNull]

namespace API_Test
{
    internal class Program
    {
        private const string socketLogs = @"C:\logSocket.txt";
        private const string clientLogs = @"C:\logClient.txt";
        private const string timeLogs = @"C:\timeClient.txt";

        private static readonly CancellationTokenSource serverTimeStartWaitToken = new CancellationTokenSource();

        private static void Main()
        {
            Console.CursorVisible = false;
            Console.WindowWidth = (int)(Console.LargestWindowWidth / 2.5);
            Console.WindowHeight = (int)(Console.LargestWindowHeight / 2.5);

            // Default Client Options
            var options = new BinanceClientHostOptions()
            {
                LogLevel = LogLevel.Debug,
                LogPath = clientLogs,
                TimeLogPath = timeLogs,
                LogToConsole = false,
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

#if DEBUG
            Json.OutputOriginalData = true;
#endif

            // API Keys need to be set to go any further
            // They don't have to be valid they just have to be set
            BaseClient.SetAuthentication("ReplaceWithYourKeysBeforeTest", "ReplaceWithYourKeysBeforeTest");

            // Select the API Endpoint Controller to use
            //BaseClient.ChangeEndpoint = ApiEndpoint.DEFAULT;
            //BaseClient.ChangeEndpoint = ApiEndpoint.TEST;
            BaseClient.ChangeEndpoint = ApiEndpoint.ONE;

            // Create a Binance Client, It will start the Server Time Client for you
            BinanceClientHost client = new BinanceClientHost(serverTimeStartWaitToken.Token);
            SocketClientHost socketClient = new SocketClientHost();

            Trace.WriteLine("Starting Test..");
            Task.Run(async () =>
            {
                // Authenticated requests will fail until the ServerTimeClient is Ready
                if (!ServerTimeClient.IsReady())
                {
                    // ServerTimeClient.WaitForStart()
                    Console.WriteLine("Server Time Client Started in " +
                        await ServerTimeClient.WaitForStart(serverTimeStartWaitToken.Token).ConfigureAwait(false) + "ms");
                }

                // Stop the Time Client
                ServerTimeClient.Stop();
                Console.WriteLine("False: " + ServerTimeClient.Exists);

                // Start The Time Client (Which will automatically wait for it to start)
                await ServerTimeClient.Start(options, serverTimeStartWaitToken.Token);
                Console.WriteLine("True: " + ServerTimeClient.Exists);

                // Ping Server
                WebCallResult<long> pingResult = client.Spot.System.PingAsync().Result;
                if (!pingResult.Success)
                {
                    Console.WriteLine("Error");
                    Console.ReadLine();
                }
                else
                {
                    Console.WriteLine("Server Ping Ticks: " + pingResult.Data.ToString());
                }

                // Get Account Status
                var status = await client.General.GetAccountStatusAsync();
                if (!status.Success)
                {
                    // Failed
                    if (status.Error.Code == -2008)
                    {
                        Console.WriteLine("Error: You didn't enter your API Keys or they are incorrect");
                    }
                    else
                    {
                        Console.WriteLine("Error: " + status.Error.ToString());
                    }
                }
                else
                {
                    // Run Tests
                    Console.WriteLine("Account Status: " + status.Data.Data.ToString());

                    ExchangeInfoTest.Run(client);

                    SocketTest.Run(socketClient);

                    OrderTest.Run(client);
                }
            });

            Console.ReadLine();
        }
    }
}
