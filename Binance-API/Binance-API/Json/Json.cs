/*
*MIT License
*
*Copyright (c) 2022 S Christison
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/

using BinanceAPI.Attributes;
using BinanceAPI.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleLog4.NET;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BinanceAPI.Logging;

namespace BinanceAPI
{
    /// <summary>
    /// Serialize / Deserialize / Debug Json
    /// This class is very different in Debug vs Release Mode and will affect performance greatly
    /// </summary>
    public class Json
    {
        /// <summary>
        /// The Serializer
        /// </summary>
        public static JsonSerializer DefaultSerializer { get; set; }

        static Json()
        {
            DefaultSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Culture = CultureInfo.InvariantCulture              
            });
        }

        internal static CallResult<JToken> ValidateJson(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                var info = "Empty data object received";
                ClientLog?.Error(info);
                return new CallResult<JToken>(null, new DeserializeError(info, data));
            }

            try
            {
                return new CallResult<JToken>(JToken.Parse(data), null);
            }
#if DEBUG
            catch (JsonReaderException jre)
            {
                var info = $"Deserialize JsonReaderException: {jre.Message}, Path: {jre.Path}, LineNumber: {jre.LineNumber}, LinePosition: {jre.LinePosition}";
                return new CallResult<JToken>(null, new DeserializeError(info, data));
            }
            catch (JsonSerializationException jse)
            {
                var info = $"Deserialize JsonSerializationException: {jse.Message}";
                return new CallResult<JToken>(null, new DeserializeError(info, data));
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToLogString();
                var info = $"Deserialize Unknown Exception: {exceptionInfo}";
                return new CallResult<JToken>(null, new DeserializeError(info, data));
            }
#else
            catch (JsonReaderException) { return new CallResult<JToken>(null, null); }
#endif
        }

        /// <summary>
        /// Deserialize a JToken into an object
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="obj">The data to deserialize</param>
        /// <param name="checkObject">Whether or not the parsing should be checked for missing properties (will output data to the logging if log verbosity is Debug)</param>
        /// <param name="requestId">Id of the request the data is returned from (used for grouping logging by request)</param>
        /// <returns></returns>
        public static CallResult<T> Deserialize<T>(JToken obj, bool? checkObject = null, int? requestId = null)
        {
            try
            {
                return new CallResult<T>(obj.ToObject<T>(DefaultSerializer), null);
            }
#if DEBUG
            catch (JsonReaderException jre)
            {
                var info = $"{(requestId != null ? $"[{requestId}] " : "")}Deserialize JsonReaderException: {jre.Message} Path: {jre.Path}, LineNumber: {jre.LineNumber}, LinePosition: {jre.LinePosition}, data: {obj}";
                ClientLog?.Error(info);
                return new CallResult<T>(default, new DeserializeError(info, obj));
            }
            catch (JsonSerializationException jse)
            {
                var info = $"{(requestId != null ? $"[{requestId}] " : "")}Deserialize JsonSerializationException: {jse.Message} data: {obj}";
                ClientLog?.Error(info);
                return new CallResult<T>(default, new DeserializeError(info, obj));
            }
            catch (Exception ex)
            {
                var exceptionInfo = ex.ToLogString();
                var info = $"{(requestId != null ? $"[{requestId}] " : "")}Deserialize Unknown Exception: {exceptionInfo}, data: {obj}";
                ClientLog?.Error(info);
                return new CallResult<T>(default, new DeserializeError(info, obj));
            }
#else
            catch (JsonReaderException) { return new CallResult<T>(default, null); }
#endif
        }

        /// <summary>
        /// Deserialize a string into an object
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="data">The data to deserialize</param>
        /// <param name="checkObject">Whether or not the parsing should be checked for missing properties (will output data to the logging if log verbosity is Debug)</param>
        /// <param name="requestId">Id of the request the data is returned from (used for grouping logging by request)</param>
        /// <returns></returns>
        public static CallResult<T> Deserialize<T>(string data, bool? checkObject = null, int? requestId = null)
        {
            var tokenResult = ValidateJson(data);
            if (!tokenResult)
            {
                ClientLog?.Error(tokenResult.Error!.Message);
                return new CallResult<T>(default, tokenResult.Error);
            }

            return Deserialize<T>(tokenResult.Data, checkObject, requestId);
        }

        /// <summary>
        /// Deserialize a stream into an object
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="stream">The stream to deserialize</param>
        /// <param name="requestId">Id of the request the data is returned from (used for grouping logging by request)</param>
        /// <returns></returns>
        public static async Task<CallResult<T>> DeserializeAsync<T>(Stream stream, int? requestId = null)
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
                return new CallResult<T>(DefaultSerializer.Deserialize<T>(jsonReader), null);
            }
#if DEBUG
            catch (JsonReaderException jre)
            {
                if (data == null && stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    data = await ReadStreamAsync(stream).ConfigureAwait(false);
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
                    data = await ReadStreamAsync(stream).ConfigureAwait(false);
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
                    data = await ReadStreamAsync(stream).ConfigureAwait(false);
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

#if DEBUG

        /// <summary>
        /// (Global) If true, the CallResult and DataEvent objects should also contain the originally received json data in the OriginalDaa property
        /// </summary>
        public static bool OutputOriginalData { get; set; }

        internal static async Task<string> ReadStreamAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, false, 512, true);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
#endif
    }
}
