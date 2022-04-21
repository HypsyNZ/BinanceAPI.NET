using BinanceAPI.Authentication;
using BinanceAPI.Objects;
using BinanceAPI.Options;
using Newtonsoft.Json;
using SimpleLog4.NET;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static BinanceAPI.Logging;

namespace BinanceAPI
{
    /// <summary>
    /// The base for all clients, websocket client and rest client
    /// </summary>
    public abstract class BaseClient : IDisposable
    {
        /// <summary>
        /// Serialize/ Deserialize / Debug Json
        /// </summary>
        internal Json Json = new();

        /// <summary>
        /// The address of the client
        /// </summary>
        public string BaseAddress { get; }

        /// <summary>
        /// The name of the exchange the client is for
        /// </summary>
        public string ExchangeName { get; }

        /// <summary>
        /// The api proxy
        /// </summary>
        protected ApiProxy? apiProxy;

        /// <summary>
        /// The authentication provider
        /// </summary>
        protected internal AuthenticationProvider? authProvider;

        /// <summary>
        /// The last used id, use NextId() to get the next id and up this
        /// </summary>
        protected static int lastId;

        /// <summary>
        /// Lock for id generating
        /// </summary>
        protected static object idLock = new();

        /// <summary>
        /// Last id used
        /// </summary>
        public static int LastId => lastId;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="exchangeName">The name of the exchange this client is for</param>
        /// <param name="options">The options for this client</param>
        /// <param name="authenticationProvider">The authentication provider for this client (can be null if no credentials are provided)</param>
        protected BaseClient(string exchangeName, ClientOptions options, AuthenticationProvider? authenticationProvider)
        {
            authProvider = authenticationProvider;
            ExchangeName = exchangeName;
            BaseAddress = options.BaseAddress;
            apiProxy = options.Proxy;
#if DEBUG
            ClientLog?.Info($"Client configuration: {options}, BinanceAPI: v{typeof(BaseClient).Assembly.GetName().Version}, {ExchangeName}.Net: v{GetType().Assembly.GetName().Version}");
            Json.ShouldCheckObjects = options.ShouldCheckObjects;
            Json.OutputOriginalData = options.OutputOriginalData;
#endif
        }

        /// <summary>
        /// Set the authentication provider, can be used when manually setting the API credentials
        /// </summary>
        /// <param name="authenticationProvider"></param>
        protected void SetAuthenticationProvider(AuthenticationProvider authenticationProvider)
        {
#if DEBUG
            ClientLog?.Info("Setting api credentials");
#endif
            authProvider = authenticationProvider;
        }

        /// <summary>
        /// Deserialize a string into an object
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="data">The data to deserialize</param>
        /// <param name="checkObject">Whether or not the parsing should be checked for missing properties (will output data to the logging if log verbosity is Debug)</param>
        /// <param name="requestId">Id of the request the data is returned from (used for grouping logging by request)</param>
        /// <returns></returns>
        public CallResult<T> Deserialize<T>(string data, bool? checkObject = null, int? requestId = null)
        {
            var tokenResult = Json.ValidateJson(data);
            if (!tokenResult)
            {
                ClientLog?.Error(tokenResult.Error!.Message);
                return new CallResult<T>(default, tokenResult.Error);
            }

            return Json.Deserialize<T>(tokenResult.Data, checkObject, requestId);
        }

        /// <summary>
        /// Deserialize a stream into an object
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="stream">The stream to deserialize</param>
        /// <param name="requestId">Id of the request the data is returned from (used for grouping logging by request)</param>
        /// <returns></returns>
        protected async Task<CallResult<T>> DeserializeAsync<T>(Stream stream, int? requestId = null)
        {
#if DEBUG
            string? data = null;
#endif
            try
            {
                // Let the reader keep the stream open so we're able to seek if needed. The calling method will close the stream.
                using var reader = new StreamReader(stream, Encoding.UTF8, false, 512, true);

#if DEBUG
                if (Json.OutputOriginalData || ClientLog?.LogLevel <= LogLevel.Debug)
                {
                    data = await reader.ReadToEndAsync().ConfigureAwait(false);
                    var result = Deserialize<T>(data, null, requestId);
                    if (Json.OutputOriginalData)
                        result.OriginalData = data;
                    return result;
                }
#endif

                using var jsonReader = new JsonTextReader(reader);
                return new CallResult<T>(Json.DefaultSerializer.Deserialize<T>(jsonReader), null);
            }
#if DEBUG
            catch (JsonReaderException jre)
            {
                if (data == null && stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    data = await Json.ReadStreamAsync(stream).ConfigureAwait(false);
                }
                else
                {
                    data = "[Data only available in Debug Mode with Debug LogLevel]";
                }

                ClientLog?.Error($"{(requestId != null ? $"[{requestId}] " : "")}Deserialize JsonReaderException: {jre.Message}, Path: {jre.Path}, LineNumber: {jre.LineNumber}, LinePosition: {jre.LinePosition}, data: {data}");
                return new CallResult<T>(default, new DeserializeError($"Deserialize JsonReaderException: {jre.Message}, Path: {jre.Path}, LineNumber: {jre.LineNumber}, LinePosition: {jre.LinePosition}", data));
            }
            catch (JsonSerializationException jse)
            {
                if (data == null && stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    data = await Json.ReadStreamAsync(stream).ConfigureAwait(false);
                }
                else
                {
                    data = "[Data only available in Debug Mode with Debug LogLevel]";
                }

                ClientLog?.Error($"{(requestId != null ? $"[{requestId}] " : "")}Deserialize JsonSerializationException: {jse.Message}, data: {data}");
                return new CallResult<T>(default, new DeserializeError($"Deserialize JsonSerializationException: {jse.Message}", data));
            }
            catch (Exception ex)
            {
                if (data == null && stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    data = await Json.ReadStreamAsync(stream).ConfigureAwait(false);
                }
                else
                {
                    data = "[Data only available in Debug Mode with Debug LogLevel]";
                }

                var exceptionInfo = ex.ToLogString();
                ClientLog?.Error($"{(requestId != null ? $"[{requestId}] " : "")}Deserialize Unknown Exception: {exceptionInfo}, data: {data}");
                return new CallResult<T>(default, new DeserializeError($"Deserialize Unknown Exception: {exceptionInfo}", data));
            }
#else
            catch (JsonReaderException) { return new CallResult<T>(default, null); }
#endif
        }

        /// <summary>
        /// Generate a new unique id. The id is staticly stored so it is guarenteed to be unique across different client instances
        /// </summary>
        /// <returns></returns>
        protected int NextId()
        {
            lock (idLock)
            {
                lastId += 1;
                return lastId;
            }
        }

        /// <summary>
        /// Fill parameters in a path. Parameters are specified by '{}' and should be specified in occuring sequence
        /// </summary>
        /// <param name="path">The total path string</param>
        /// <param name="values">The values to fill</param>
        /// <returns></returns>
        protected static string FillPathParameter(string path, params string[] values)
        {
            foreach (var value in values)
            {
                var index = path.IndexOf("{}", StringComparison.Ordinal);
                if (index >= 0)
                {
                    path = path.Remove(index, 2);
                    path = path.Insert(index, value);
                }
            }
            return path;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            authProvider?.Credentials?.Dispose();
#if DEBUG
            ClientLog?.Info("Disposing exchange client");
#endif
        }
    }
}