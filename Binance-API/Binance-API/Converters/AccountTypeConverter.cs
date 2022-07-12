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

using BinanceAPI.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BinanceAPI.Converters
{
    /// <summary>
    /// Account Type Converter
    /// </summary>
    public class AccountTypeConverter : JsonConverter
    {
        /// <summary>
        /// Account Type Converter
        /// </summary>
        public AccountTypeConverter()
        {

        }

        /// <summary>
        /// Known String Mappings
        /// </summary>
        protected List<KeyValuePair<AccountType, string>> Mapping => new()
        {
            new KeyValuePair<AccountType, string>(AccountType.Unknown, "UNKNOWN"),
            new KeyValuePair<AccountType, string>(AccountType.Spot, "SPOT"),
            new KeyValuePair<AccountType, string>(AccountType.Margin, "MARGIN"),
            new KeyValuePair<AccountType, string>(AccountType.Futures, "FUTURES"),
            new KeyValuePair<AccountType, string>(AccountType.Leveraged, "LEVERAGED"),
            new KeyValuePair<AccountType, string>(AccountType.TRD_GRP_002, "TRD_GRP_002"),
            new KeyValuePair<AccountType, string>(AccountType.TRD_GRP_003, "TRD_GRP_003"),
            new KeyValuePair<AccountType, string>(AccountType.TRD_GRP_003, "TRD_GRP_004")
        };

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString().ToUpper() ?? "UNKNOWN");
        }

        /// <inheritdoc />
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return AccountType.Unknown;
            }

            var rValue = reader.Value.ToString();

            var mapping = Mapping.FirstOrDefault(kv => kv.Value.Equals(rValue, StringComparison.InvariantCulture));

            if (mapping.Equals(default(KeyValuePair<AccountType, string>)))
            {
                mapping = Mapping.FirstOrDefault(kv => kv.Value.Equals(rValue, StringComparison.InvariantCultureIgnoreCase));
            }

            if (!mapping.Equals(default(KeyValuePair<AccountType, string>)))
            {
                return mapping.Key;
            }
            else
            {
                Debug.WriteLine($"Cannot map enum. Type: {typeof(AccountType)}, Value: {reader.Value}");
                return AccountType.Unknown;
            }
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            // Check if it is type, or nullable of type
            return objectType == typeof(AccountType) || Nullable.GetUnderlyingType(objectType) == typeof(AccountType);
        }
    }
}
