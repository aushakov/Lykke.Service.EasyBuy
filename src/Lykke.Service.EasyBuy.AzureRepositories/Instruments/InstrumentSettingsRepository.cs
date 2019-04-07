using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;
using Lykke.Service.EasyBuy.Domain.Repositories;

namespace Lykke.Service.EasyBuy.AzureRepositories.Instruments
{
    public class InstrumentSettingsRepository : IInstrumentSettingsRepository
    {
        private readonly INoSQLTableStorage<InstrumentSettingsEntity> _storage;

        public InstrumentSettingsRepository(INoSQLTableStorage<InstrumentSettingsEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyList<InstrumentSettings>> GetAllAsync()
        {
            IEnumerable<InstrumentSettingsEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<InstrumentSettings>>(entities);
        }

        public async Task InsertAsync(InstrumentSettings instrumentSettings)
        {
            var entity = new InstrumentSettingsEntity(GetPartitionKey(), GetRowKey(instrumentSettings.AssetPair));

            Mapper.Map(instrumentSettings, entity);

            await _storage.InsertAsync(entity);
        }

        public async Task UpdateAsync(InstrumentSettings instrumentSettings)
        {
            await _storage.MergeAsync(GetPartitionKey(), GetRowKey(instrumentSettings.AssetPair), entity =>
            {
                Mapper.Map(instrumentSettings, entity);
                return entity;
            });
        }

        public Task DeleteAsync(string assetPair)
        {
            return _storage.DeleteAsync(GetPartitionKey(), GetRowKey(assetPair));
        }

        private static string GetPartitionKey()
            => "Instrument";

        private static string GetRowKey(string assetPair)
            => assetPair;
    }
}
