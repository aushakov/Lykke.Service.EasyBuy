using System;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Settings;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface ISettingsService
    {
        string GetInstanceName();

        string GetWalletId();

        TimeSpan GetRecalculationInterval();

        Task<TimersSettings> GetTimersSettingsAsync();

        Task UpdateTimersSettingsAsync(TimersSettings timersSettings);
    }
}
