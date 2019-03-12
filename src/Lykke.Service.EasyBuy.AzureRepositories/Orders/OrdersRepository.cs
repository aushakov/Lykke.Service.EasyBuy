using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.EasyBuy.AzureRepositories.Orders
{
    [UsedImplicitly]
    public class OrdersRepository : IOrdersRepository
    {
        private readonly INoSQLTableStorage<OrderEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _indices;

        public OrdersRepository(
            INoSQLTableStorage<OrderEntity> storage,
            INoSQLTableStorage<AzureIndex> indices)
        {
            _storage = storage;
            _indices = indices;
        }
        
        public async Task InsertAsync(Order order)
        {
            var entity = new OrderEntity(GetPartitionKey(order.WalletId), GetRowKey(order.Id));

            Mapper.Map(order, entity);

            await _storage.InsertThrowConflictAsync(entity);
            
            var index = new AzureIndex(GetIndexPartitionKey(order.Status), GetIndexRowKey(order.Id), entity);
            
            await _indices.InsertAsync(index);
        }

        public async Task UpdateAsync(Order order)
        {
            var entity = await _storage.GetDataAsync(GetPartitionKey(order.WalletId), GetRowKey(order.Id));

            OrderStatus? oldStatus = entity.Status != order.Status ? entity.Status : default(OrderStatus?);
            
            Mapper.Map(order, entity);

            await _storage.InsertOrMergeAsync(entity);

            if (oldStatus.HasValue)
            {
                await _indices.DeleteAsync(GetIndexPartitionKey(oldStatus.Value), GetIndexRowKey(order.Id));
                
                var index = new AzureIndex(GetIndexPartitionKey(order.Status), GetIndexRowKey(order.Id), entity);
            
                await _indices.InsertAsync(index);
            }
        }

        public async Task<IReadOnlyList<Order>> GetAllAsync(string walletId, string assetPair, DateTime? timeFrom, DateTime? timeTo, int limit)
        {
            var query = new TableQuery<OrderEntity>().Where(BuildQuery(walletId, assetPair, timeFrom, timeTo)).Take(limit);

            var entities = await _storage.WhereAsync(query);

            return Mapper.Map<IReadOnlyList<Order>>(entities);
        }

        public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status)
        {
            var indices = await _indices.GetDataAsync(GetIndexPartitionKey(status));

            var entities = await _storage.GetDataAsync(indices);

            return Mapper.Map<List<Order>>(entities);
        }

        public async Task<Order> GetAsync(string walletId, string orderId)
        {
            return Mapper.Map<Order>(await _storage.GetDataAsync(GetPartitionKey(walletId), GetRowKey(orderId)));
        }

        private static string BuildQuery(string walletId, string assetPair, DateTime? timeFrom, DateTime? timeTo)
        {
            var filterBuilder = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(walletId))
            {
                filterBuilder.Append(TableQuery.GenerateFilterCondition(nameof(OrderEntity.PartitionKey), QueryComparisons.Equal,
                    GetPartitionKey(walletId)));
            }
            else
            {
                var partitionKeysToIgnore = Enum.GetNames(typeof(OrderStatus));
                
                filterBuilder.Append(TableQuery.GenerateFilterCondition(nameof(OrderEntity.PartitionKey), QueryComparisons.NotEqual,
                    partitionKeysToIgnore[0]));

                for (var i = 1; i < partitionKeysToIgnore.Length; i++)
                {
                    filterBuilder.Append($" {TableOperators.And} {TableQuery.GenerateFilterCondition(nameof(OrderEntity.PartitionKey), QueryComparisons.NotEqual, partitionKeysToIgnore[i])}");
                }
            }
            
            if (!string.IsNullOrWhiteSpace(assetPair))
            {
                filterBuilder.Append($" {TableOperators.And} {TableQuery.GenerateFilterCondition(nameof(OrderEntity.AssetPair), QueryComparisons.Equal, assetPair)}");
            }
            
            if (timeFrom != null)
            {
                filterBuilder.Append($" {TableOperators.And} {TableQuery.GenerateFilterConditionForDate(nameof(OrderEntity.CreatedTime), QueryComparisons.GreaterThanOrEqual, new DateTimeOffset(timeFrom.Value))}");
            }
            
            if (timeTo != null)
            {
                filterBuilder.Append($" {TableOperators.And} {TableQuery.GenerateFilterConditionForDate(nameof(OrderEntity.CreatedTime), QueryComparisons.LessThanOrEqual, new DateTimeOffset(timeTo.Value))}");
            }

            return filterBuilder.ToString();
        }
        
        private static string GetPartitionKey(string walletId)
            => walletId;

        private static string GetRowKey(string id)
            => id;
        
        private static string GetIndexPartitionKey(OrderStatus status)
            => status.ToString();

        private static string GetIndexRowKey(string orderId)
            => orderId;
    }
}
