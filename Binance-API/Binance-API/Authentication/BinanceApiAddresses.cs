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

namespace BinanceAPI.Authentication
{
    /// <summary>
    /// Api addresses
    /// </summary>
    public class BinanceApiAddresses
    {
        /// <summary>
        /// The address used by the BinanceClient for the Spot API
        /// </summary>
        public string RestClientAddress { get; set; } = "";

        /// <summary>
        /// The address used by the BinanceSocketClient for the Spot API
        /// </summary>
        public string SocketClientAddress { get; set; } = "";

        /// <summary>
        /// The default addresses to connect to the binance.com API
        /// </summary>
        public static BinanceApiAddresses Default = new()
        {
            RestClientAddress = "https://api.binance.com",
            SocketClientAddress = "wss://stream.binance.com:9443/",
        };

        /// <summary>
        /// The addresses to connect to the binance testnet
        /// </summary>
        public static BinanceApiAddresses TestNet = new()
        {
            RestClientAddress = "https://testnet.binance.vision",
            SocketClientAddress = "wss://testnet.binance.vision",
        };

        /// <summary>
        /// The addresses to connect to binance.us.
        /// </summary>
        public static BinanceApiAddresses Us = new()
        {
            RestClientAddress = "https://api.binance.us",
            SocketClientAddress = "wss://stream.binance.us:9443",
        };
    }
}
