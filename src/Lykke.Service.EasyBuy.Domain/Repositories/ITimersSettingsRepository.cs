using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Settings;

namespace Lykke.Service.EasyBuy.Domain.Repositories
{
    public interface ITimersSettingsRepository
    {
        Task<TimersSettings> GetAsync();

        Task InsertOrReplaceAsync(TimersSettings timersSettings);
    }
}
