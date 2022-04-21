namespace BinanceAPI.Objects
{
    /// <summary>
    /// No api credentials provided while trying to access a private endpoint
    /// </summary>
    public class NoApiCredentialsError : Error
    {
        /// <summary>
        /// ctor
        /// </summary>
        public NoApiCredentialsError() : base(null, "No credentials provided for private endpoint", null) { }
    }
}