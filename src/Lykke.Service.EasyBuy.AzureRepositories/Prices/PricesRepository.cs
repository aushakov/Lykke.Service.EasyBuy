using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Repositories;

namespace Lykke.Service.EasyBuy.AzureRepositories.Prices
{
    public class PricesRepository : IPricesRepository
    {
        private readonly INoSQLTableStorage<PriceEntity> _storage;

        public PricesRepository(INoSQLTableStorage<PriceEntity> storage)
        {
            _storage = storage;
        }
        
        public async Task<Price> GetAsync(string id)
        {
            return Mapper.Map<Price>(await _storage.GetDataAsync(GetPartitionKey(), GetRowKey(id)));
        }

        public async Task<Price> GetLatestAsync(string assetPair, OrderType type)
        {
            return Mapper.Map<Price>(
                (await _storage.GetTopRecordsAsync(
                    GetLatestPartitionKey(assetPair, type), 1)).SingleOrDefault());
        }

        public async Task InsertAsync(Price price)
        {
            var entity = new PriceEntity(GetPartitionKey(), GetRowKey(price.Id));

            Mapper.Map(price, entity);

            await _storage.InsertThrowConflictAsync(entity);
            
            var latestEntity = new PriceEntity(
                GetLatestPartitionKey(price.AssetPair, price.Type),
                GetLatestRowKey(price.ValidFrom));
            
            Mapper.Map(price, latestEntity);

            await _storage.InsertThrowConflictAsync(latestEntity);
        }
        
        private static string GetPartitionKey()
            => "Price";

        private static string GetRowKey(string id)
            => id;

        private static string GetLatestPartitionKey(string assetPair, OrderType type)
            => $"{assetPair}_{type.ToString()}";
        
        private static string GetLatestRowKey(DateTime validFrom)
            => DateTime
                .MaxValue
                .Subtract(validFrom)
                .TotalMilliseconds
                .ToString(CultureInfo.InvariantCulture);
    }
}
