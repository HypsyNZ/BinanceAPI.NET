namespace BinanceAPI.Objects
{
    /// <summary>
    /// Cancellation requested
    /// </summary>
    public class CancellationRequestedError : Error
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CancellationRequestedError() : base(null, "Cancellation requested", null) { }
    }
}