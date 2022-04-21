using BinanceAPI.Interfaces;
using System.Collections.Generic;

namespace BinanceAPI.Sockets
{
    /// <summary>
    /// Default weboscket factory implementation
    /// </summary>
    public class WebsocketFactory : IWebsocketFactory
    {
        /// <inheritdoc />
        public IWebsocket CreateWebsocket(string url)
        {
            return new WebSocketClient(url);
        }

        /// <inheritdoc />
        public IWebsocket CreateWebsocket(string url, IDictionary<string, string> cookies, IDictionary<string, string> headers)
        {
            return new WebSocketClient(url, cookies, headers);
        }
    }
}