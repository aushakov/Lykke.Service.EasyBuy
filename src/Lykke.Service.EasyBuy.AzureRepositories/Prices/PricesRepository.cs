using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.EasyBuy.Domain.Entities.Prices;
using Lykke.Service.EasyBuy.Domain.Repositories;

namespace Lykke.Service.EasyBuy.AzureRepositories.Prices
{
    public class PriceRepository : IPriceRepository
    {
        private readonly INoSQLTableStorage<PriceEntity> _storage;

        public PriceRepository(INoSQLTableStorage<PriceEntity> storage)
        {
            _storage = storage;
        }

        public async Task<Price> GetByIdAsync(string priceId)
        {
            PriceEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey(priceId));

            return Mapper.Map<Price>(entity);
        }

        public async Task<IReadOnlyList<Price>> GetLatestAsync()
        {
            IEnumerable<PriceEntity> entities = await _storage.GetDataAsync(GetLatestPartitionKey());

            return Mapper.Map<List<Price>>(entities);
        }

        public async Task InsertAsync(Price price)
        {
            var entity = new PriceEntity(GetPartitionKey(), GetRowKey(price.Id));

            Mapper.Map(price, entity);

            await _storage.InsertAsync(entity);

            var latestEntity = new PriceEntity(GetLatestPartitionKey(), GetLatestRowKey(price.AssetPair));

            Mapper.Map(price, latestEntity);

            await _storage.InsertOrReplaceAsync(latestEntity);
        }

        private static string GetPartitionKey()
            => "Price";

        private static string GetRowKey(string id)
            => id;

        private static string GetLatestPartitionKey()
            => "LatestPrice";

        private static string GetLatestRowKey(string assetPair)
            => assetPair;
    }
}
