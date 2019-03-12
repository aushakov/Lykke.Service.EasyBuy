using System.Threading.Tasks;

namespace Lykke.Service.EasyBuy.Domain.Repositories
{
    public interface IDefaultSettingsRepository
    {
        Task<DefaultSetting> GetAsync();
        
        Task CreateOrUpdateAsync(DefaultSetting setting);
    }
}
