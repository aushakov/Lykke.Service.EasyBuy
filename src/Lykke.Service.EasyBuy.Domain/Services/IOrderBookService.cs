using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.OrderBooks;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface IOrderBookService
    {
        OrderBook GetByAssetPairId(string exchange, string assetPairId);

        IReadOnlyList<string> GetExistingExchanges();

        Task HandleAsync(OrderBook orderBook);
    }
}
