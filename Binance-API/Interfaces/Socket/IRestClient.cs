using BinanceAPI.Objects;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceAPI.Interfaces
{
    /// <summary>
    /// Base class for rest API implementations
    /// </summary>
    public interface IRestClient : IDisposable
    {
        /// <summary>
        /// The factory for creating requests. Used for unit testing
        /// </summary>
        IRequestFactory RequestFactory { get; set; }

        /// <summary>
        /// The total amount of requests made
        /// </summary>
        int TotalRequestsMade { get; }

        /// <summary>
        /// The base address of the API
        /// </summary>
        string BaseAddress { get; }

        /// <summary>
        /// Client name
        /// </summary>
        string ExchangeName { get; }

        /// <summary>
        /// Ping to see if the server is reachable
        /// </summary>
        /// <returns>The roundtrip time of the ping request</returns>
        CallResult<long> Ping(CancellationToken ct = default);

        /// <summary>
        /// Ping to see if the server is reachable
        /// </summary>
        /// <returns>The roundtrip time of the ping request</returns>
        Task<CallResult<long>> PingAsync(CancellationToken ct = default);
    }
}