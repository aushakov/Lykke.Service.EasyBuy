using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Domain.Entities.Settings;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class SettingsService : ISettingsService
    {
        private readonly string _instanceName;
        private readonly string _walletId;
        private readonly TimeSpan _recalculationInterval;
        private readonly ITimersSettingsRepository _timersSettingsRepository;

        private TimersSettings _timersSettings;

        public SettingsService(
            string instanceName,
            string walletId,
            TimeSpan recalculationInterval,
            ITimersSettingsRepository timersSettingsRepository)
        {
            _instanceName = instanceName;
            _walletId = walletId;
            _recalculationInterval = recalculationInterval;
            _timersSettingsRepository = timersSettingsRepository;
        }

        public string GetInstanceName()
            => _instanceName;

        public string GetWalletId()
            => _walletId;

        public TimeSpan GetRecalculationInterval()
            => _recalculationInterval;

        public async Task<TimersSettings> GetTimersSettingsAsync()
        {
            if (_timersSettings == null)
            {
                _timersSettings = await _timersSettingsRepository.GetAsync();

                if (_timersSettings == null)
                {
                    _timersSettings = new TimersSettings
                    {
                        Orders = TimeSpan.FromSeconds(5)
                    };
                }
            }

            return _timersSettings;
        }

        public async Task UpdateTimersSettingsAsync(TimersSettings timersSettings)
        {
            await _timersSettingsRepository.InsertOrReplaceAsync(timersSettings);

            _timersSettings = timersSettings;
        }
    }
}
