using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.EasyBuy.Domain;

namespace Lykke.Service.EasyBuy.AzureRepositories.Prices
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateAlways)]
    public class PriceEntity : AzureTableEntity
    {
        public PriceEntity()
        {
        }

        public PriceEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        
        public string Id { set; get; }
        
        public string AssetPair { set; get; }
        
        public OrderType Type { set; get; }
        
        public decimal Value { set; get; }
        
        public decimal BaseVolume { set; get; }
        
        public decimal QuotingVolume { set; get; }
        
        public decimal Markup { set; get; }
        
        public decimal OriginalPrice { set; get; }
        
        public string Exchange { set; get; }
        
        public DateTime ValidFrom { set; get; }
        
        public DateTime ValidTo { set; get; }
        
        public TimeSpan AllowedOverlap { set; get; }
    }
}
