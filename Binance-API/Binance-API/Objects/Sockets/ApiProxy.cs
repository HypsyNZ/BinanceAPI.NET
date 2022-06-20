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

using System.Security;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// Proxy info
    /// </summary>
    public class ApiProxy
    {
        /// <summary>
        /// The host address of the proxy
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// The port of the proxy
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// The login of the proxy
        /// </summary>
        public string? Login { get; }

        /// <summary>
        /// The password of the proxy
        /// </summary>
        public SecureString? Password { get; }

        /// <summary>
        /// Create new settings for a proxy
        /// </summary>
        /// <param name="host">The proxy hostname/ip</param>
        /// <param name="port">The proxy port</param>
        public ApiProxy(string host, int port) : this(host, port, null, (SecureString?)null)
        {
        }

        /// <summary>
        /// Create new settings for a proxy
        /// </summary>
        /// <param name="host">The proxy hostname/ip</param>
        /// <param name="port">The proxy port</param>
        /// <param name="login">The proxy login</param>
        /// <param name="password">The proxy password</param>
        public ApiProxy(string host, int port, string? login, string? password) : this(host, port, login, password?.ToSecureString())
        {
        }

        /// <summary>
        /// Create new settings for a proxy
        /// </summary>
        /// <param name="host">The proxy hostname/ip</param>
        /// <param name="port">The proxy port</param>
        /// <param name="login">The proxy login</param>
        /// <param name="password">The proxy password</param>
        public ApiProxy(string host, int port, string? login, SecureString? password)
        {
            Host = host;
            Port = port;
            Login = login;
            Password = password;
        }
    }
}
