using BinanceAPI.Authentication;
using BinanceAPI.Objects;

namespace BinanceAPI.Options
{
    /// <summary>
    /// Base client options
    /// </summary>
    public class ClientOptions : BaseOptions
    {
        private string _baseAddress = "";

        /// <summary>
        /// The base address of the client
        /// </summary>
        public string BaseAddress
        {
            get => _baseAddress;
            set
            {
                var newValue = value;
                if (!newValue.EndsWith("/"))
                    newValue += "/";
                _baseAddress = newValue;
            }
        }

        /// <summary>
        /// The api credentials
        /// </summary>
        public ApiCredentials? ApiCredentials { get; set; }

        /// <summary>
        /// Should check objects for missing properties based on the model and the received JSON
        /// </summary>
        public bool ShouldCheckObjects { get; set; } = false;

        /// <summary>
        /// Proxy to use
        /// </summary>
        public ApiProxy? Proxy { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="baseAddress">The base address to use</param>
        public ClientOptions(string baseAddress)
        {
            BaseAddress = baseAddress;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()}, Credentials: {(ApiCredentials == null ? "-" : "Set")}, BaseAddress: {BaseAddress}, Proxy: {(Proxy == null ? "-" : Proxy.Host)}";
        }
    }
}