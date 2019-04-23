using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.EasyBuy.Client.Models.Instruments;
using Refit;

namespace Lykke.Service.EasyBuy.Client.Api
{
    /// <summary>
    /// Provides methods to work with instruments.
    /// </summary>
    [PublicAPI]
    public interface IInstrumentsApi
    {
        /// <summary>
        /// Returns a collection of instruments.
        /// </summary>
        /// <returns>A collection of instruments.</returns>
        [Get("/api/instruments")]
        Task<IReadOnlyList<InstrumentModel>> GetAllAsync();

        /// <summary>
        /// Returns an instrument by asset pair identifier.
        /// </summary>
        /// <param name="assetPair">The identifier of asses pair.</param>
        /// <returns>An instrument.</returns>
        /// <exception cref="ClientApiException">If instrument does not exist.</exception>
        [Get("/api/instruments/{assetPair}")]
        Task<InstrumentModel> GetByAssetPairAsync(string assetPair);

        /// <summary>
        /// Adds new instrument settings.
        /// </summary>
        /// <param name="model">The model that describes instrument.</param>
        /// <exception cref="ClientApiException">If instrument already exists.</exception>
        [Post("/api/instruments")]
        Task AddAsync([Body] InstrumentModel model);

        /// <summary>
        /// Updates instrument settings.
        /// </summary>
        /// <param name="model">The model that describes instrument.</param>
        /// <exception cref="ClientApiException">If instrument does not exist.</exception>
        [Put("/api/instruments")]
        Task UpdateAsync([Body] InstrumentModel model);

        /// <summary>
        /// Deletes the instrument by identifier.
        /// </summary>
        /// <param name="instrumentId">The identifier of instrument.</param>
        /// <exception cref="ClientApiException">If instrument does not exist.</exception>
        [Delete("/api/instruments/{instrumentId}")]
        Task DeleteAsync(string instrumentId);
    }
}
