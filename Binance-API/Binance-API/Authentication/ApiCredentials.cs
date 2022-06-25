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
            return new ApiCredentials(Key!.GetString(), Secret!.GetString());
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
        }
    }
}
