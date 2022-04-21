namespace BinanceAPI.Objects
{
    /// <summary>
    /// An invalid parameter has been provided
    /// </summary>
    public class ArgumentError : Error
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message"></param>
        public ArgumentError(string message) : base(null, "Invalid parameter: " + message, null) { }
    }
}