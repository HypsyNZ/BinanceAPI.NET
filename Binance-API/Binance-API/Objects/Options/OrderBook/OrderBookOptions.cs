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

using SimpleLog4.NET;

namespace BinanceAPI.Options
{
    /// <summary>
    /// Base for order book options
    /// </summary>
    public class OrderBookOptions : BaseOptions
    {
        /// <summary>
        /// The name of the order book implementation
        /// </summary>
        public string OrderBookName { get; }

        /// <summary>
        /// Whether or not checksum validation is enabled. Default is true, disabling will ignore checksum messages.
        /// </summary>
        public bool ChecksumValidationEnabled { get; set; } = true;

        /// <summary>
        /// Whether each update should have a consecutive id number. Used to identify and reconnect when numbers are skipped.
        /// </summary>
        public bool SequenceNumbersAreConsecutive { get; }

        /// <summary>
        /// Whether or not a level should be removed from the book when it's pushed out of scope of the limit. For example with a book of limit 10,
        /// when a new bid level is added which makes the total amount of bids 11, should the last bid entry be removed
        /// </summary>
        public bool StrictLevels { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name">The name of the order book implementation</param>
        /// <param name="sequencesAreConsecutive">Whether each update should have a consecutive id number. Used to identify and reconnect when numbers are skipped.</param>
        /// <param name="strictLevels">Whether or not a level should be removed from the book when it's pushed out of scope of the limit. For example with a book of limit 10,
        /// when a new bid is added which makes the total amount of bids 11, should the last bid entry be removed</param>
        /// <param name="logPath">Path to the log</param>
        /// <param name="logLevel">Log level for the log</param>
        /// <param name="logToConsole">Ture if messages should also be logged to the console</param>
        public OrderBookOptions(string name, bool sequencesAreConsecutive, bool strictLevels, string logPath, LogLevel logLevel, bool logToConsole)
        {
            OrderBookName = name;
            SequenceNumbersAreConsecutive = sequencesAreConsecutive;
            StrictLevels = strictLevels;
            LogPath = logPath;
            LogLevel = logLevel;
            LogToConsole = logToConsole;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}, OrderBookName: {OrderBookName}, SequenceNumbersAreConsequtive: {SequenceNumbersAreConsecutive}, StrictLevels: {StrictLevels}";
        }
    }
}
