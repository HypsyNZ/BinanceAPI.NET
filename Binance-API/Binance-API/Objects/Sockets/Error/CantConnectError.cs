namespace BinanceAPI.Objects
{
    /// <summary>
    /// Cant reach server error
    /// </summary>
    public class CantConnectError : Error
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CantConnectError() : base(null, "Can't connect to the server", null) { }
    }
}