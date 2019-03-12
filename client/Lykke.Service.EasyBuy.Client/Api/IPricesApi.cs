using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Client.Models;
using Refit;

namespace Lykke.Service.EasyBuy.Client.Api
{
    /// <summary>
    /// Provides method to work with prices.
    /// </summary>
    [PublicAPI]
    public interface IPricesApi
    {
        /// <summary>
        /// Used to return calculated price by client and price ids.
        /// </summary>
        /// <param name="priceId">Price Id.</param>
        /// <returns></returns>
        [Get("/api/prices/{priceId}")]
        Task<PriceModel> GetPriceAsync(string priceId);
        
        /// <summary>
        /// Used to return all actual prices for active instruments by type.
        /// </summary>
        /// <param name="type">Types of prices.</param>
        /// <returns></returns>
        [Get("/api/prices/actual")]
        Task<IReadOnlyList<PriceModel>> GetActualPricesAsync(OrderType type);
    }
}
