namespace BinanceAPI.Objects
{
    /// <summary>
    /// Invalid operation requested
    /// </summary>
    public class InvalidOperationError : Error
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message"></param>
        public InvalidOperationError(string message) : base(null, message, null) { }
    }
}