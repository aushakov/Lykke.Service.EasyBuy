using System;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;

namespace Lykke.Service.EasyBuy.AzureRepositories.Instruments
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class InstrumentSettingsEntity : AzureTableEntity
    {
        private TimeSpan _priceLifetime;
        private TimeSpan _overlapTime;
        private decimal _markup;
        private decimal _volume;
        private InstrumentStatus _status;

        public InstrumentSettingsEntity()
        {
        }

        public InstrumentSettingsEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string AssetPair { set; get; }

        public string Exchange { set; get; }

        public TimeSpan PriceLifetime
        {
            get => _priceLifetime;
            set
            {
                if (_priceLifetime != value)
                {
                    _priceLifetime = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public TimeSpan OverlapTime
        {
            get => _overlapTime;
            set
            {
                if (_overlapTime != value)
                {
                    _overlapTime = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Markup
        {
            get => _markup;
            set
            {
                if (_markup != value)
                {
                    _markup = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public decimal Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }

        public InstrumentStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    MarkValueTypePropertyAsDirty();
                }
            }
        }
    }
}
