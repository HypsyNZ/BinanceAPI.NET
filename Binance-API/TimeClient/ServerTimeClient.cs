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

using BinanceAPI.Objects;
using BinanceAPI.Objects.Spot.MarketData;
using BinanceAPI.Time;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.Clients
{
    /// <summary>
    /// The <see href="ServerTimeClient"/>
    /// </summary>
    public static class ServerTimeClient
    {
        private const int PING_DIVIDED_BY_2 = 5000;
        private static bool _guesserRanToCompletion = false;

        private static BinanceClientHost? Client { get; set; }
        private static Stopwatch ServerTimeOffset = new Stopwatch();
        private static BinanceClientHostOptions? options;
        private static readonly object ExistsLock = new object();
        private static readonly object UpdateLock = new object();

        /// <summary>
        /// Increments when the time went out of sync, caused an error and needed to be corrected
        /// <para>This value won't reset until you restart your application</para>
        /// </summary>
        public static int CorrectionCount { get; internal set; }

        /// <summary>
        /// Missed Pings, These will be ignored and are counted to troubleshoot connection issues
        /// <para>This value won't reset until you restart your application</para>
        /// </summary>
        public static int MissedPingCount { get; private set; }

        /// <summary>
        /// The number of times the Guesser has Ran to Completion
        /// </summary>
        public static int GuesserAttemptCount { get; private set; }

        /// <summary>
        /// True if an instance of the Server Time Client should already exist
        /// </summary>
        public static bool Exists { get; private set; }

        /// <summary>
        /// Your Ping Divided By 2 Last Time You Synced The Time
        /// </summary>
        public static long PingTicks { get; private set; }

        /// <summary>
        /// The Current Remote Time Reported By The Server
        /// </summary>
        public static long RemoteTime { get; private set; }

        /// <summary>
        /// The Current Guess Offset
        /// </summary>
        public static long GuessOffset { get; private set; }

        /// <summary>
        /// True when the <see href="ServerTimeClient"/> is Ready
        /// <para>Authenticated Requests will fail until Ready is true</para>
        /// </summary>
        public static bool IsReady()
        {
            if (ServerTimeTicks != 0 && _guesserRanToCompletion)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The Current Server Time Guess in Ticks
        /// </summary>
        public static long ServerTimeTicks => RemoteTime + ServerTimeOffset.ElapsedTicks;

        /// <summary>
        /// Cancel the Server Time Updater Loop
        /// </summary>
        public static CancellationTokenSource LoopToken = new CancellationTokenSource();

        /// <summary>
        /// Start the <see href="ServerTimeClient"/>
        /// </summary>
        /// <param name="clientOptions"></param>
        /// <param name="waitToken">Cancellation token for the Start Wait</param>
        public static async Task Start(BinanceClientHostOptions clientOptions, CancellationToken waitToken)
        {
            if (!Exists)
            {
                lock (ExistsLock)
                {
                    if (Exists)
                    {
                        return;
                    }

                    Exists = true;
                }

                Exists = true;
                options = clientOptions;

                Logging.StartTimeLog(options.TimeLogPath, options.LogLevel, options.LogToConsole);
                Logging.TimeLog?.Debug("Started Time Client: " + DateTime.UtcNow);

                if (Client != null)
                {
                    return;
                }

                Client = new(options, default);

                _ = Task.Factory.StartNew(async (o) =>
                 {
                     PingTicks = Client.Ping().Data * PING_DIVIDED_BY_2;
                     RemoteTime = GetServerTimeTicksAsync().Result;
                     ServerTimeOffset.Start();
                     await Guesser().ConfigureAwait(false);
                     while (!LoopToken.Token.IsCancellationRequested)
                     {
                         try
                         {
                             await Guesser().ConfigureAwait(false);
                             _guesserRanToCompletion = true;
                             await Task.Delay(TimeSpan.FromMinutes(options.SyncUpdateTime)).ConfigureAwait(false);
                         }
                         catch (Exception ex)
                         {
                             Logging.TimeLog?.Error(ex);
                         }
                     }
                 }, TaskCreationOptions.DenyChildAttach).ConfigureAwait(false);
                await WaitForStart(waitToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Stop the Server Time Client
        /// </summary>
        public static void Stop()
        {
            if (Exists)
            {
                LoopToken.Cancel();
                Exists = false;
                LoopToken = new CancellationTokenSource();
            }
        }

        /// <summary>
        /// Wait for the Server Time Client to Start
        /// </summary>
        /// <param name="token">Cancellation token for timeout etc</param>
        /// <returns>The amount of time the Server Time Client took to Start in Milliseconds</returns>
        public static async Task<long> WaitForStart(CancellationToken token = default)
        {
            Stopwatch sw = Stopwatch.StartNew();

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(2).ConfigureAwait(false);
                if (IsReady())
                {
                    break;
                }
            }

            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Guess the Server Time
        /// </summary>
        /// <returns></returns>
        public static Task Guesser()
        {
            lock (UpdateLock)
            {
                var ping = Client!.Ping().Data * PING_DIVIDED_BY_2;
                if (ping > 0)
                {
                    PingTicks = ping;
                }
                else
                {
                    MissedPingCount++;
                }

                var currentGuess = ServerTimeTicks;
                RemoteTime = GetServerTimeTicksAsync().Result;
                ServerTimeOffset.Restart();

                GuessOffset = RemoteTime - currentGuess;
            }

#if DEBUG
            Logging.TimeLog?.Debug("Server Time Guesser Completed");
            var guess = GetRequestTimestampLong();
            var serverTime = RemoteTime + ServerTimeOffset.ElapsedTicks;
            var guessAheadBy = (guess - serverTime) / 10000;
            Logging.TimeLog?.Debug("Guesser: " + guess.ToString() + " | ServerTime: " + serverTime.ToString() + "| GuesserAheadBy: " + guessAheadBy);
#endif

            GuesserAttemptCount++;
            return Task.CompletedTask;
        }

        /// <summary>
        /// The Current Server Time as Reported By The Server
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task<long> GetServerTimeTicksAsync(CancellationToken ct = default)
        {
            var parameters = new Dictionary<string, object>();

            var result = await Client!.SendRequestInternal<BinanceCheckTime>(UriManager.API_ONE.General.API_V3_TIME_SyncTime, HttpMethod.Get, ct, parameters);
            if (!result)
            {
                return 0;
            }

            return result.Data.ServerTime.Ticks;
        }

        /// <summary>
        /// Get the current Server Time from the <see href="ServerTimeClient"/> for use in a Request
        /// <para>This value is not directly retrieved from the Server, It is a derived Guess</para>
        /// </summary>
        /// <returns>Will return an empty string if the <see href="ServerTimeClient"/> is missing</returns>
        internal static string GetRequestTimestamp()
        {
            lock (UpdateLock)
            {
                return TimeHelper.ToUnixTimestamp(ServerTimeTicks + GuessOffset + PingTicks).ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Get the current Server Time from the <see href="ServerTimeClient"/> for use in a Request
        /// <para>This value is not directly retrieved from the Server, It is a derived Guess</para>
        /// </summary>
        /// <returns>Will return an empty string if the <see href="ServerTimeClient"/> is missing</returns>
        public static long GetRequestTimestampLong()
        {
            lock (UpdateLock)
            {
                return ServerTimeTicks + GuessOffset + PingTicks;
            }
        }
    }
}
