using BinanceAPI.Interfaces.SubClients;
using BinanceAPI.Interfaces.SubClients.Margin;
using BinanceAPI.Interfaces.SubClients.Spot;

namespace BinanceAPI.Interfaces
{
    /// <summary>
    /// Binance interface
    /// </summary>
    public interface IBinanceClient : IRestClient
    {
        /// <summary>
        /// Spot endpoints
        /// </summary>
        IBinanceClientSpot Spot { get; }

        /// <summary>
        /// Margin endpoints (Isolated) 
        /// </summary>
        IBinanceClientMargin Margin { get; }

        /// <summary>
        /// Fiat endpoints
        /// </summary>
        IBinanceClientFiat Fiat { get; set; }

        /// <summary>
        /// General endpoints
        /// </summary>
        IBinanceClientGeneral General { get; }

        /// <summary>
        /// Lending endpoints
        /// </summary>
        IBinanceClientLending Lending { get; }

        /// <summary>
        /// Withdraw/Deposit endpoints
        /// </summary>
        IBinanceClientWithdrawDeposit WithdrawDeposit { get; }

        /// <summary>
        /// Set the API key and secret
        /// </summary>
        /// <param name="apiKey">The api key</param>
        /// <param name="apiSecret">The api secret</param>
        void SetApiCredentials(string apiKey, string apiSecret);
    }
}