﻿using BinanceAPI.Attributes;
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
        internal JsonSerializer DefaultSerializer { get; set; }

#if DEBUG
        /// <summary>
        /// (Global) If true, the CallResult and DataEvent objects should also contain the originally received json data in the OriginalDaa property
        /// </summary>
        public static bool OutputOriginalData { get; set; }

        /// <summary>
        /// (Global) Should check objects for missing properties based on the model and the received JSON
        /// </summary>
        public static bool ShouldCheckObjects { get; set; }

#endif

        /// <summary>
        /// Create a Json Serializer/Deserializer
        /// </summary>
        public Json()
        {
            DefaultSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Culture = CultureInfo.InvariantCulture
            });
        }

        /// <summary>
        /// Deserialize a JToken into an object
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="obj">The data to deserialize</param>
        /// <param name="checkObject">Whether or not the parsing should be checked for missing properties (will output data to the logging if log verbosity is Debug)</param>
        /// <param name="requestId">Id of the request the data is returned from (used for grouping logging by request)</param>
        /// <returns></returns>
        public CallResult<T> Deserialize<T>(JToken obj, bool? checkObject = null, int? requestId = null)
        {
            try
            {
#if DEBUG
                if ((checkObject ?? ShouldCheckObjects) && ClientLog?.LogLevel <= LogLevel.Debug)
                {
                    // This checks the input JToken object against the class it is being serialized into and outputs any missing fields
                    // in either the input or the class
                    if (obj is JObject o)
                    {
                        CheckObject(typeof(T), o, requestId);
                    }
                    else if (obj is JArray j)
                    {
                        if (j.HasValues && j[0] is JObject jObject)
                            CheckObject(typeof(T).GetElementType(), jObject, requestId);
                    }
                }
#endif
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

        internal CallResult<JToken> ValidateJson(string data)
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

#if DEBUG

        private PropertyInfo? GetProperty(string name, IEnumerable<PropertyInfo> props)
        {
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false).FirstOrDefault();
                if (attr == null)
                {
                    if (string.Equals(prop.Name, name, StringComparison.CurrentCultureIgnoreCase))
                        return prop;
                }
                else
                {
                    if (((JsonPropertyAttribute)attr).PropertyName == name)
                        return prop;
                }
            }
            return null;
        }

        private void CheckObject(Type type, JObject obj, int? requestId = null)
        {
            if (type == null)
                return;

            if (type.GetCustomAttribute<JsonConverterAttribute>(true) != null)
                // If type has a custom JsonConverter we assume this will handle property mapping
                return;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                return;

            if (!obj.HasValues && type != typeof(object))
            {
                ClientLog?.Warning($"{(requestId != null ? $"[{requestId}] " : "")}Expected `{type.Name}`, but received object was empty");

                return;
            }

            var isDif = false;

            var properties = new List<string>();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttributes(typeof(JsonPropertyAttribute), false).FirstOrDefault();
                var ignore = prop.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).FirstOrDefault();
                if (ignore != null)
                    continue;

                var propertyName = ((JsonPropertyAttribute?)attr)?.PropertyName;
                properties.Add(propertyName ?? prop.Name);
            }
            foreach (var token in obj)
            {
                var d = properties.FirstOrDefault(p => p == token.Key);
                if (d == null)
                {
                    d = properties.SingleOrDefault(p => string.Equals(p, token.Key, StringComparison.CurrentCultureIgnoreCase));
                    if (d == null)
                    {
                        if (!(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                        {
                            ClientLog?.Warning($"{(requestId != null ? $"[{requestId}] " : "")}Local object doesn't have property `{token.Key}` expected in type `{type.Name}`");
                            isDif = true;
                        }

                        continue;
                    }
                }

                properties.Remove(d);

                var propType = GetProperty(d, props)?.PropertyType;
                if (propType == null || token.Value == null)
                    continue;
                if (!IsSimple(propType) && propType != typeof(DateTime))
                {
                    if (propType.IsArray && token.Value.HasValues && ((JArray)token.Value).Any() && ((JArray)token.Value)[0] is JObject)
                        CheckObject(propType.GetElementType()!, (JObject)token.Value[0]!, requestId);
                    else if (token.Value is JObject o)
                        CheckObject(propType, o, requestId);
                }
            }

            foreach (var prop in properties)
            {
                var propInfo = props.First(p => p.Name == prop ||
                    ((JsonPropertyAttribute)p.GetCustomAttributes(typeof(JsonPropertyAttribute), false).FirstOrDefault())?.PropertyName == prop);
                var optional = propInfo.GetCustomAttributes(typeof(JsonOptionalPropertyAttribute), false).FirstOrDefault();
                if (optional != null)
                    continue;

                isDif = true;
                ClientLog?.Warning($"{(requestId != null ? $"[{requestId}] " : "")}Local object has property `{prop}` but was not found in received object of type `{type.Name}`");
            }

            if (isDif)
                ClientLog?.Info($"{(requestId != null ? $"[{ requestId}] " : "")}Returned data: " + obj);
        }

        private bool IsSimple(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal);
        }

        internal async Task<string> ReadStreamAsync(Stream stream)
        {
            using var reader = new StreamReader(stream, Encoding.UTF8, false, 512, true);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

#endif
    }
}