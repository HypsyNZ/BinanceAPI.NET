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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BinanceAPI.Converters
{
    /// <summary>
    /// Base class for enum converters
    /// </summary>
    /// <typeparam name="T">Type of enum to convert</typeparam>
    public abstract class BaseConverter<T> : JsonConverter where T : struct
    {
        /// <summary>
        /// The enum->string mapping
        /// </summary>
        protected abstract List<KeyValuePair<T, string>> Mapping { get; }

        private readonly bool quotes;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="useQuotes"></param>
        protected BaseConverter(bool useQuotes)
        {
            quotes = useQuotes;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var stringValue = value == null ? null : GetValue((T)value);
            if (quotes)
                writer.WriteValue(stringValue);
            else
                writer.WriteRawValue(stringValue);
        }

        /// <inheritdoc />
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;

            if (!GetValue(reader.Value.ToString(), out var result))
            {
                Debug.WriteLine($"Cannot map enum. Type: {typeof(T)}, Value: {reader.Value}");
                return null;
            }

            return result;
        }

        /// <summary>
        /// Convert a string value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public T ReadString(string data)
        {
            return Mapping.FirstOrDefault(v => v.Value == data).Key;
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            // Check if it is type, or nullable of type
            return objectType == typeof(T) || Nullable.GetUnderlyingType(objectType) == typeof(T);
        }

        private bool GetValue(string value, out T result)
        {
            //check for exact match first, then if not found fallback to a case insensitive match
            var mapping = Mapping.FirstOrDefault(kv => kv.Value.Equals(value, StringComparison.InvariantCulture));
            if (mapping.Equals(default(KeyValuePair<T, string>)))
                mapping = Mapping.FirstOrDefault(kv => kv.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase));

            if (!mapping.Equals(default(KeyValuePair<T, string>)))
            {
                result = mapping.Key;
                return true;
            }

            result = default;
            return false;
        }

        private string GetValue(T value)
        {
            return Mapping.FirstOrDefault(v => v.Key.Equals(value)).Value;
        }
    }
}
