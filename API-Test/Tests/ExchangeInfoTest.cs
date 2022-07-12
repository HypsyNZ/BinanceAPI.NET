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

using BinanceAPI.ClientHosts;
using BinanceAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API_Test
{
    // EXCHANGE INFO
    internal class ExchangeInfoTest
    {
        public static void Run(BinanceClientHost client)
        {
            _ = Task.Run(async () =>
            {
                var result = await client.Spot.System.GetExchangeInfoAsync().ConfigureAwait(false);

                if (result.Success)
                {
                    Console.WriteLine("Passed loaded exchange info for: [" + result.Data.Symbols.Count() + "] symbols");
                    Console.WriteLine("Rate: " + result.Data.RateLimits.First().Interval.ToString() + " | Limit: " + result.Data.RateLimits.First().Limit.ToString());

                    var r = result.Data.Symbols.Where(t => t.Permissions != null).FirstOrDefault();
                    Console.WriteLine(r.Name);
                    foreach (var p in r.Permissions)
                    {
                        Console.WriteLine(p);
                    }


                    Console.WriteLine("Spot: " + AccountType.Spot.ToString());
                    Console.WriteLine("Spot: " + nameof(AccountType.Spot));


                    var se = JsonConvert.SerializeObject(result.Data);

                    //_ = true; // Breakpoint;

                    var de = JsonConvert.DeserializeObject(se);

                    Console.WriteLine(de);



                }
            }).ConfigureAwait(false);
        }
    }
}
