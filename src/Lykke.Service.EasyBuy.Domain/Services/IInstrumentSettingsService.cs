using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface IInstrumentSettingsService
    {
        Task<IReadOnlyList<InstrumentSettings>> GetAllAsync();

        Task<IReadOnlyList<InstrumentSettings>> GetActiveAsync();

        Task<InstrumentSettings> GetByAssetPairAsync(string assetPair);

        Task AddAsync(InstrumentSettings instrumentSettings);

        Task UpdateAsync(InstrumentSettings instrumentSettings);

        Task DeleteAsync(string assetPair);
    }
}
