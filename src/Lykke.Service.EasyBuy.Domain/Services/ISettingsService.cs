using System.Threading.Tasks;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface ISettingsService
    {
        Task<string> GetServiceInstanceNameAsync();
        Task<string> GetWalletIdAsync();
        Task<DefaultSetting> GetDefaultSettingsAsync();
        Task UpdateDefaultSettingsAsync(DefaultSetting defaultSettings);
    }
}
