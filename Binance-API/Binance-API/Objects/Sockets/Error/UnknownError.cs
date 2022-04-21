namespace BinanceAPI.Objects
{
    /// <summary>
    /// Unknown error
    /// </summary>
    public class UnknownError : Error
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="data">Error data</param>
        public UnknownError(string message, object? data = null) : base(null, message, data) { }
    }
}