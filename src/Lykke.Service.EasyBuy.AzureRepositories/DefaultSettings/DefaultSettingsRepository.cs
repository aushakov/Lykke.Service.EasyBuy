using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.Service.EasyBuy.Domain;

namespace Lykke.Service.EasyBuy.AzureRepositories.DefaultSettings
{
    public class DefaultSettingsRepository : IDefaultSettingsRepository
    {
        private readonly INoSQLTableStorage<DefaultSettingsEntity> _storage;

        public DefaultSettingsRepository(INoSQLTableStorage<DefaultSettingsEntity> storage)
        {
            _storage = storage;
        }
        
        public async Task<DefaultSetting> GetAsync()
        {
            return Mapper.Map<DefaultSetting>(await _storage.GetDataAsync(GetPartitionKey(), GetRowKey()));
        }

        public async Task CreateOrUpdateAsync(DefaultSetting setting)
        {
            var entity = new DefaultSettingsEntity(GetPartitionKey(), GetRowKey());

            Mapper.Map(setting, entity);

            await _storage.InsertOrMergeAsync(entity);
        }

        private static string GetPartitionKey() => "partition";

        private static string GetRowKey() => "row";
    }
}
