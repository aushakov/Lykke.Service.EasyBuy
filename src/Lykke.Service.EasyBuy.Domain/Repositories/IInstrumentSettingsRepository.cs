using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;

namespace Lykke.Service.EasyBuy.Domain.Repositories
{
    public interface IInstrumentSettingsRepository
    {
        Task<IReadOnlyList<InstrumentSettings>> GetAllAsync();

        Task InsertAsync(InstrumentSettings instrumentSettings);

        Task UpdateAsync(InstrumentSettings instrumentSettings);

        Task DeleteAsync(string assetPair);
    }
}
