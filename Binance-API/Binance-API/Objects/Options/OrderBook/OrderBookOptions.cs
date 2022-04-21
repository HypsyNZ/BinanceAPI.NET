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
        public OrderBookOptions(string name, bool sequencesAreConsecutive, bool strictLevels, string logPath, LogLevel logLevel)
        {
            OrderBookName = name;
            SequenceNumbersAreConsecutive = sequencesAreConsecutive;
            StrictLevels = strictLevels;
            LogPath = logPath;
            LogLevel = logLevel;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}, OrderBookName: {OrderBookName}, SequenceNumbersAreConsequtive: {SequenceNumbersAreConsecutive}, StrictLevels: {StrictLevels}";
        }
    }
}