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
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace BinanceAPI.Sockets
{
    internal class PendingRequest
    {
        private readonly CancellationTokenSource cts;

        public Func<JToken, bool> Handler { get; }
        public JToken Result { get; private set; }
        public bool Completed { get; private set; }
        public AsyncResetEvent Event { get; }
        public TimeSpan Timeout { get; }

        public PendingRequest(Func<JToken, bool> handler, TimeSpan timeout)
        {
            Handler = handler;
            Event = new AsyncResetEvent(false, false);
            Timeout = timeout;

            cts = new CancellationTokenSource(timeout);
            cts.Token.Register(Fail, false);
        }

        public bool CheckData(JToken data)
        {
            if (Handler(data))
            {
                Result = data;
                Completed = true;
                Event.Set();
                return true;
            }

            return false;
        }

        public void Fail()
        {
            Completed = true;
            Event.Set();
        }
    }
}
