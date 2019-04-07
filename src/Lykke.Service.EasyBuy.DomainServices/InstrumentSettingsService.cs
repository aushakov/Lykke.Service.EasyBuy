using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.Service.EasyBuy.Domain.Services;
using Lykke.Service.EasyBuy.DomainServices.Cache;
using Lykke.Service.EasyBuy.DomainServices.Extensions;

namespace Lykke.Service.EasyBuy.DomainServices
{
    public class InstrumentSettingsService : IInstrumentSettingsService
    {
        private readonly IInstrumentSettingsRepository _instrumentSettingsRepository;
        private readonly ILog _log;
        private readonly InMemoryCache<InstrumentSettings> _cache;

        public InstrumentSettingsService(
            IInstrumentSettingsRepository instrumentSettingsRepository,
            ILogFactory logFactory)
        {
            _instrumentSettingsRepository = instrumentSettingsRepository;
            _log = logFactory.CreateLog(this);
            _cache = new InMemoryCache<InstrumentSettings>(GetKey, false);
        }

        public async Task<IReadOnlyList<InstrumentSettings>> GetAllAsync()
        {
            IReadOnlyList<InstrumentSettings> instrumentsSettings = _cache.GetAll();

            if (instrumentsSettings == null)
            {
                instrumentsSettings = await _instrumentSettingsRepository.GetAllAsync();

                _cache.Initialize(instrumentsSettings);
            }

            return instrumentsSettings;
        }

        public async Task<IReadOnlyList<InstrumentSettings>> GetActiveAsync()
        {
            IReadOnlyList<InstrumentSettings> instrumentSettings = await GetAllAsync();

            return instrumentSettings.Where(o => o.Status == InstrumentStatus.Active).ToList();
        }

        public async Task<InstrumentSettings> GetByAssetPairAsync(string assetPair)
        {
            IReadOnlyList<InstrumentSettings> instrumentSettings = await GetAllAsync();

            return instrumentSettings.FirstOrDefault(o => o.AssetPair == assetPair);
        }

        public async Task AddAsync(InstrumentSettings instrumentSettings)
        {
            InstrumentSettings currentInstrumentSettings = await GetByAssetPairAsync(instrumentSettings.AssetPair);

            if (currentInstrumentSettings != null)
                throw new EntityAlreadyExistsException();

            await _instrumentSettingsRepository.InsertAsync(instrumentSettings);

            _cache.Set(instrumentSettings);

            _log.InfoWithDetails("Instrument settings added", instrumentSettings);
        }

        public async Task UpdateAsync(InstrumentSettings instrumentSettings)
        {
            InstrumentSettings currentInstrumentSettings = await GetByAssetPairAsync(instrumentSettings.AssetPair);

            if (currentInstrumentSettings == null)
                throw new EntityNotFoundException();

            await _instrumentSettingsRepository.UpdateAsync(instrumentSettings);

            _cache.Set(instrumentSettings);

            _log.InfoWithDetails("Instrument settings updated", instrumentSettings);
        }

        public async Task DeleteAsync(string assetPair)
        {
            InstrumentSettings instrumentSettings = await GetByAssetPairAsync(assetPair);

            if (instrumentSettings == null)
                throw new EntityNotFoundException();

            await _instrumentSettingsRepository.DeleteAsync(assetPair);

            _cache.Remove(GetKey(assetPair));

            _log.InfoWithDetails("Instrument settings deleted", instrumentSettings);
        }

        private static string GetKey(InstrumentSettings instrumentSettings)
            => GetKey(instrumentSettings.AssetPair);

        private static string GetKey(string assetPair)
            => assetPair;
    }
}
