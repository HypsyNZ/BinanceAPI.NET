namespace BinanceAPI.Objects
{
    /// <summary>
    /// Error while deserializing data
    /// </summary>
    public class DeserializeError : Error
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="data">The data which caused the error</param>
        public DeserializeError(string message, object? data) : base(null, message, data) { }
    }
}