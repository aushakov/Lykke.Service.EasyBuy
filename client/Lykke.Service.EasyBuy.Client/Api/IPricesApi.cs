using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Client.Models.Prices;
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
        Task<PriceModel> GetByIdAsync(string priceId);

        /// <summary>
        /// Used to return all actual prices for active instruments.
        /// </summary>
        /// <returns></returns>
        [Get("/api/prices")]
        Task<IReadOnlyList<PriceModel>> GetAllAsync();
    }
}
