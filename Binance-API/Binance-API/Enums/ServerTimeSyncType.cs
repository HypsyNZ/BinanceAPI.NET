namespace BinanceAPI.Enums
{
    /// <summary>
    /// Server Time Update Mode
    /// <para><see cref="Manual"/> Manually Specify AutoTimestampStartTime and AutoTimestampTime in the Client Options</para>
    /// <para><see cref="Minute"/> Sync the Server Time Every Minute</para>
    /// <para><see cref="Hourly"/> Sync the Server Time Once an Hour</para>
    /// <para><see cref="Aggressive"/> Sync the Server time every 125ms</para>
    /// </summary>
    public enum ServerTimeSyncType
    {
        /// <summary>
        /// Manually Specify AutoTimestampStartTime and AutoTimestampTime in the Client Options
        /// </summary>
        Manual,

        /// <summary>
        /// Sync the Server Time every Minute
        /// </summary>>
        Minute,

        /// <summary>
        /// Sync the Server time Once an Hour
        /// </summary>
        Hourly,

        /// <summary>
        /// Sync the Server time every 125ms
        /// </summary>
        Aggressive,
    }
}