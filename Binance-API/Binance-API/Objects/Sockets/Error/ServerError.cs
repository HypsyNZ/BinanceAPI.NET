namespace BinanceAPI.Objects
{
    /// <summary>
    /// Error returned by the server
    /// </summary>
    public class ServerError : Error
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public ServerError(string message, object? data = null) : base(null, message, data) { }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public ServerError(int code, string message, object? data = null) : base(code, message, data)
        {
        }
    }
}