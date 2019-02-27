using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface IOrderBookService
    {
        OrderBook GetByAssetPairId(string exchange, string assetPairId);

        IReadOnlyList<string> GetExistingExchanges();

        Task HandleAsync(string exchange, OrderBook orderBook);
    }
}
