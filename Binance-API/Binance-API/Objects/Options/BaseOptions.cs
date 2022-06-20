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
    /// Base options
    /// </summary>
    public class BaseOptions
    {
        /// <summary>
        /// The minimum log level to output. Setting it to null will send all messages to the registered ILoggers.
        /// <para>Default Information</para>
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// The Path to the log for this client
        /// <para>This is set by the first SocketClient</para>
        /// </summary>
        public string LogPath { get; set; } = string.Empty;

        /// <summary>
        /// True if messages should also be logged to the console
        /// <para>Default True</para>
        /// </summary>
        public bool LogToConsole { get; set; } = true;

        /// <summary>
        /// If true, the CallResult and DataEvent objects will also include the originally received json data in the OriginalData property
        /// <para>Default false</para>
        /// </summary>
        public bool OutputOriginalData { get; set; } = false;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"LogLevel: {LogLevel}, OutputOriginalData: {OutputOriginalData}";
        }
    }
}
