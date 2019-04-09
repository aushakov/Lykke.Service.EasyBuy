using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Entities.OrderBooks;
using Lykke.Service.EasyBuy.Domain.Services;
using Lykke.Service.EasyBuy.DomainServices.Cache;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class OrderBookService : IOrderBookService
    {
        private readonly InMemoryCache<OrderBook> _cache;

        public OrderBookService()
        {
            _cache = new InMemoryCache<OrderBook>(GetKey, true);
        }

        public IReadOnlyList<string> GetExistingExchanges()
        {
            return _cache.GetAll().Select(o => o.Exchange).ToList();
        }

        public Task HandleAsync(OrderBook orderBook)
        {
            // TODO: Validate order book

            _cache.Set(orderBook);

            return Task.CompletedTask;
        }

        public OrderBook GetByAssetPairId(string exchange, string assetPair)
        {
            return _cache.Get(GetKey(exchange, assetPair));
        }

        private static string GetKey(OrderBook orderBook)
            => GetKey(orderBook.Exchange, orderBook.AssetPair);

        private static string GetKey(string exchange, string assetPair)
            => $"{exchange}_{assetPair}";
    }
}
