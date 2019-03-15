using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Refit;

namespace Lykke.Service.EasyBuy.Client.Api
{
    /// <summary>
    /// Provides methods to work with exchanges.
    /// </summary>
    [PublicAPI]
    public interface IExchangesApi
    {
        /// <summary>
        /// Provides ability to request available exchanges.
        /// </summary>
        /// <returns>A list of available exchanges.</returns>
        [Get("/api/exchanges/available")]
        Task<IReadOnlyList<string>> GetAvailableAsync();
    }
}
