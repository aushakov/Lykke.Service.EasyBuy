using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EasyBuy.Domain.Entities.Trades;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.EasyBuy.AzureRepositories.Trades
{
    [UsedImplicitly]
    public class TradeRepository : ITradeRepository
    {
        private readonly INoSQLTableStorage<TradeEntity> _storage;

        public TradeRepository(INoSQLTableStorage<TradeEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<Trade>> GetAsync(DateTime from, DateTime to)
        {
            string filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(AzureTableEntity.PartitionKey), QueryComparisons.GreaterThan,
                    GetPartitionKey(to.Date.AddDays(1))),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(AzureTableEntity.PartitionKey), QueryComparisons.LessThan,
                    GetPartitionKey(from.Date.AddMilliseconds(-1))));

            var query = new TableQuery<TradeEntity>().Where(filter);

            IEnumerable<TradeEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<IReadOnlyList<Trade>>(entities);
        }

        public async Task InsertAsync(Trade trade)
        {
            var entity = new TradeEntity(GetPartitionKey(trade.CreationDateTime), GetRowKey(trade.Id));

            Mapper.Map(trade, entity);

            await _storage.InsertAsync(entity);
        }

        private static string GetPartitionKey(DateTime time)
            => (DateTime.MaxValue.Ticks - time.Date.Ticks).ToString("D19");

        private static string GetRowKey(string orderId)
            => orderId;
    }
}
