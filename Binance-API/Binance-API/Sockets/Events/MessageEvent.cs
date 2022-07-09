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

using Newtonsoft.Json.Linq;
using System;

namespace BinanceAPI.Sockets
{
    /// <summary>
    /// Message received event
    /// </summary>
    public class MessageEvent
    {
        /// <summary>
        /// The json object of the data
        /// </summary>
        public JToken JsonData { get; set; }

        /// <summary>
        /// The originally received string data
        /// </summary>
        public string? OriginalData { get; set; }

        /// <summary>
        /// The timestamp of when the data was received
        /// </summary>
        public DateTime ReceivedTimestamp { get; set; }

        /// <summary>
        /// The Message
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="originalData"></param>
        /// <param name="timestamp"></param>
        public MessageEvent(JToken jsonData, string? originalData, DateTime timestamp)
        {
            JsonData = jsonData;
            OriginalData = originalData;
            ReceivedTimestamp = timestamp;
        }
    }
}
