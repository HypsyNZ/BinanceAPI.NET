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
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// The Path to the log for this client
        /// <para>This is set by the first SocketClient</para>
        /// </summary>
        public string LogPath { get; set; } = string.Empty;

        /// <summary>
        /// If true, the CallResult and DataEvent objects will also include the originally received json data in the OriginalData property
        /// </summary>
        public bool OutputOriginalData { get; set; } = false;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"LogLevel: {LogLevel}, OutputOriginalData: {OutputOriginalData}";
        }
    }
}