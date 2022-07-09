using BinanceAPI.ClientBase;
using BinanceAPI.Objects;

namespace BinanceAPI.Options
{
    /// <summary>
    /// Base client options
    /// </summary>
    public class ClientOptions : BaseOptions
    {
        /// <summary>
        /// Should check objects for missing properties based on the model and the received JSON
        /// </summary>
        public bool ShouldCheckObjects { get; set; } = false;

        /// <summary>
        /// Proxy to use
        /// </summary>
        public ApiProxy? Proxy { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}, Credentials: {(!BaseClient.IsAuthenticationSet ? "Not Set" : "Set")}, BaseAddress: {UriClient.GetBaseAddress()}, Proxy: {(Proxy == null ? "Not Set" : Proxy.Host)}";
        }
    }
}