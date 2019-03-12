using System.Threading.Tasks;

namespace Lykke.Service.EasyBuy.Domain.Repositories
{
    public interface IPricesRepository
    {   
        Task<Price> GetAsync(string id);

        Task<Price> GetLatestAsync(string assetPair, OrderType type);
        
        Task InsertAsync(Price price);
    }
}
