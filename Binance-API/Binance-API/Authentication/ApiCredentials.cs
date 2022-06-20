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

using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Security;
using System.Text;

namespace BinanceAPI.Authentication
{
    /// <summary>
    /// Api credentials info
    /// </summary>
    public class ApiCredentials : IDisposable
    {
        /// <summary>
        /// The api key to authenticate requests
        /// </summary>
        public SecureString? Key { get; }

        /// <summary>
        /// The api secret to authenticate requests
        /// </summary>
        public SecureString? Secret { get; }

        /// <summary>
        /// The private key to authenticate requests
        /// </summary>
        public PrivateKey? PrivateKey { get; }

        /// <summary>
        /// Create Api credentials providing a private key for authentication
        /// </summary>
        /// <param name="privateKey">The private key used for signing</param>
        public ApiCredentials(PrivateKey privateKey)
        {
            PrivateKey = privateKey;
        }

        /// <summary>
        /// Create Api credentials providing an api key and secret for authentication
        /// </summary>
        /// <param name="key">The api key used for identification</param>
        /// <param name="secret">The api secret used for signing</param>
        public ApiCredentials(SecureString key, SecureString secret)
        {
            if (key == null || secret == null)
                throw new ArgumentException("Key and secret can't be null/empty");

            Key = key;
            Secret = secret;
        }

        /// <summary>
        /// Create Api credentials providing an api key and secret for authentication
        /// WILL AUTOMATICALLY BE CONVERTED INTO A SECURE STRING
        /// </summary>
        /// <param name="key">The api key used for identification</param>
        /// <param name="secret">The api secret used for signing</param>
        public ApiCredentials(string key, string secret)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(secret))
                throw new ArgumentException("Key and secret can't be null/empty");

            Key = key.ToSecureString();
            Secret = secret.ToSecureString();
        }

        /// <summary>
        /// Copy the credentials
        /// </summary>
        /// <returns></returns>
        public ApiCredentials Copy()
        {
            if (PrivateKey == null)
                return new ApiCredentials(Key!.GetString(), Secret!.GetString());
            else
                return new ApiCredentials(PrivateKey!.Copy());
        }

        /// <summary>
        /// Create Api credentials providing a stream containing json data. The json data should include two values: apiKey and apiSecret
        /// </summary>
        /// <param name="inputStream">The stream containing the json data</param>
        /// <param name="identifierKey">A key to identify the credentials for the API. For example, when set to `binanceKey` the json data should contain a value for the property `binanceKey`. Defaults to 'apiKey'.</param>
        /// <param name="identifierSecret">A key to identify the credentials for the API. For example, when set to `binanceSecret` the json data should contain a value for the property `binanceSecret`. Defaults to 'apiSecret'.</param>
        public ApiCredentials(Stream inputStream, string? identifierKey = null, string? identifierSecret = null)
        {
            using var reader = new StreamReader(inputStream, Encoding.UTF8, false, 512, true);

            var stringData = reader.ReadToEnd();
            var jsonData = stringData.ToJToken();
            if (jsonData == null)
                throw new ArgumentException("Input stream not valid json data");

            var key = TryGetValue(jsonData, identifierKey ?? "apiKey");
            var secret = TryGetValue(jsonData, identifierSecret ?? "apiSecret");

            if (key == null || secret == null)
                throw new ArgumentException("apiKey or apiSecret value not found in Json credential file");

            Key = key.ToSecureString();
            Secret = secret.ToSecureString();

            inputStream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Try get the value of a key from a JToken
        /// </summary>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string? TryGetValue(JToken data, string key)
        {
            if (data[key] == null)
                return null;
            return (string)data[key]!;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Key?.Dispose();
            Secret?.Dispose();
            PrivateKey?.Dispose();
        }
    }
}
