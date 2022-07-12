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

using BinanceAPI.Authentication;
using BinanceAPI.Objects;
using BinanceAPI.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using static BinanceAPI.Logging;

namespace BinanceAPI.ClientBase
{
    /// <summary>
    /// The base for all clients, websocket client and rest client
    /// </summary>
    public abstract class BaseClient : IDisposable
    {
        private static int lastId;
        private static object idLock = new();
        private static ApiEndpoint _changeEndpoint = ApiEndpoint.ONE;
        protected private static AuthenticationProvider _authProvider;

        [AllowNull]
        protected private ApiProxy ApiProxy;

        /// <summary>
        /// Has the the Authentication Provider been set
        /// </summary>
        public static bool IsAuthenticationSet { get; private set; } = false;

        /// <summary>
        /// Change which API Endpoint Controller to use for Requests
        /// <para>Default is API1</para>
        /// </summary>
        public static ApiEndpoint ChangeEndpoint { get => _changeEndpoint; set => _changeEndpoint = value; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="options">The options for this client</param>
        protected BaseClient(ClientOptions options)
        {
            if (!IsAuthenticationSet)
            {
                throw new InvalidOperationException("You must set your API Keys with BaseClient.SetAuthentication() first");
            }

            ApiProxy = options.Proxy;

            ClientLog?.Info($"Client configuration: {options}, BinanceAPI.NET: v{GetType().Assembly.GetName().Version}");
#if DEBUG
            Json.OutputOriginalData = options.OutputOriginalData;
#endif
        }

        /// <summary>
        /// Set API Credentials to be used for Authentication
        /// <para>The strings will be converted into Secure Strings</para>
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiSecret"></param>
        public static void SetAuthentication(string apiKey, string apiSecret)
        {
#if DEBUG
            ClientLog?.Info("Converting strings to Secure Strings and Setting API Credentials");
#endif
            _authProvider = new AuthenticationProvider(new ApiCredentials(apiKey, apiSecret));
            IsAuthenticationSet = true;
        }

        /// <summary>
        /// Set API Credentials to be used for Authentication using Secure Strings
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="apiSecret"></param>
        public static void SetAuthentication(SecureString apiKey, SecureString apiSecret)
        {
#if DEBUG
            ClientLog?.Info("Setting API Credentials using Secure Strings");
#endif
            _authProvider = new AuthenticationProvider(new ApiCredentials(apiKey, apiSecret));
            IsAuthenticationSet = true;
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
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            _authProvider?.Dispose();
#if DEBUG
            ClientLog?.Info("Disposing exchange client");
#endif
        }
    }
}
