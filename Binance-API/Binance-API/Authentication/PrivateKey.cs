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

using System;
using System.Security;

namespace BinanceAPI.Authentication
{
    /// <summary>
    /// Private key info
    /// </summary>
    public class PrivateKey : IDisposable
    {
        /// <summary>
        /// The private key
        /// </summary>
        public SecureString Key { get; }

        /// <summary>
        /// The private key's pass phrase
        /// </summary>
        public SecureString? Passphrase { get; }

        /// <summary>
        /// Indicates if the private key is encrypted or not
        /// </summary>
        public bool IsEncrypted { get; }

        /// <summary>
        /// Create a private key providing an encrypted key information
        /// </summary>
        /// <param name="key">The private key used for signing</param>
        /// <param name="passphrase">The private key's passphrase</param>
        public PrivateKey(SecureString key, SecureString passphrase)
        {
            Key = key;
            Passphrase = passphrase;

            IsEncrypted = true;
        }

        /// <summary>
        /// Create a private key providing an encrypted key information
        /// </summary>
        /// <param name="key">The private key used for signing</param>
        /// <param name="passphrase">The private key's passphrase</param>
        public PrivateKey(string key, string passphrase)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(passphrase))
                throw new ArgumentException("Key and passphrase can't be null/empty");

            var secureKey = new SecureString();
            foreach (var c in key)
                secureKey.AppendChar(c);
            secureKey.MakeReadOnly();
            Key = secureKey;

            var securePassphrase = new SecureString();
            foreach (var c in passphrase)
                securePassphrase.AppendChar(c);
            securePassphrase.MakeReadOnly();
            Passphrase = securePassphrase;

            IsEncrypted = true;
        }

        /// <summary>
        /// Create a private key providing an unencrypted key information
        /// </summary>
        /// <param name="key">The private key used for signing</param>
        public PrivateKey(SecureString key)
        {
            Key = key;

            IsEncrypted = false;
        }

        /// <summary>
        /// Create a private key providing an encrypted key information
        /// </summary>
        /// <param name="key">The private key used for signing</param>
        public PrivateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key can't be null/empty");

            Key = key.ToSecureString();

            IsEncrypted = false;
        }

        /// <summary>
        /// Copy the private key
        /// </summary>
        /// <returns></returns>
        public PrivateKey Copy()
        {
            if (Passphrase == null)
                return new PrivateKey(Key.GetString());
            else
                return new PrivateKey(Key.GetString(), Passphrase.GetString());
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Key?.Dispose();
            Passphrase?.Dispose();
        }
    }
}
