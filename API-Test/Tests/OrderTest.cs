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
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace API_Test
{
    // ORDER TEST
    internal class OrderTest
    {
        public static void Run(BinanceClientHost client)
        {
            _ = Task.Run(async () =>
            {
                DateTime testStartTime = DateTime.UtcNow + TimeSpan.FromMinutes(10);
                Stopwatch testTime = Stopwatch.StartNew();
                while (true)
                {
                    if (testStartTime < DateTime.UtcNow)
                    {
                        Console.WriteLine("Passed");
                        return;
                    }

                    await Task.Delay(200).ConfigureAwait(false);

                    // Guesser Test
                    var serverTime = ServerTimeClient.GetServerTimeTicksAsync().Result;
                    var guess = ServerTimeClient.GetRequestTimestampLong();
                    var guessAheadBy = (guess - serverTime) / 10000;
                    // Guesser Test

                    Console.WriteLine("Guess: " + guess.ToString() + " | ServerTime: " + serverTime.ToString() + "| GuessAheadBy: " + guessAheadBy);

                    if (guessAheadBy < 1000 && guessAheadBy > -1000)
                    {
                        var result = await client.Spot.Order.PlaceTestOrderAsync("BTCUSDT", OrderSide.Sell, OrderType.Market, quantity: 123).ConfigureAwait(false);
                        if (!result.Success)
                        {
                            Console.WriteLine("Order Failed: " + result.Error.ToString());
                        }
                        else
                        {
                            Console.WriteLine(result.Success.ToString() + "| MissedPings: " + ServerTimeClient.MissedPingCount + "| CorrectionAttempts: " + ServerTimeClient.CorrectionCount + "| GuesserRanToCompletion: " + ServerTimeClient.GuesserAttemptCount);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ahead/Behind Server Time by 1000ms");
                        return;
                    }
                }
            }).ConfigureAwait(false);
        }

    }
}
