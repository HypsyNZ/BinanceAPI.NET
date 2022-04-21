namespace BinanceAPI.Objects
{
    /// <summary>
    /// Status of the order book
    /// </summary>
    public enum OrderBookStatus
    {
        /// <summary>
        /// Not connected
        /// </summary>
        Disconnected,

        /// <summary>
        /// Connecting
        /// </summary>
        Connecting,

        /// <summary>
        /// Reconnecting
        /// </summary>
        Reconnecting,

        /// <summary>
        /// Syncing data
        /// </summary>
        Syncing,

        /// <summary>
        /// Data synced, order book is up to date
        /// </summary>
        Synced
    }
}