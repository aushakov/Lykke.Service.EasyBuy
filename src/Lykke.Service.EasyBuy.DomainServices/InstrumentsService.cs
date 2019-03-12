using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class InstrumentsService : IInstrumentsService
    {
        private readonly IInstrumentsAccessService _instrumentsAccessService;
        private readonly IPricesGenerator _pricesGenerator;

        public InstrumentsService(
            IInstrumentsAccessService instrumentsAccessService,
            IPricesGenerator pricesGenerator)
        {
            _instrumentsAccessService = instrumentsAccessService;
            _pricesGenerator = pricesGenerator;
        }

        public Task<IReadOnlyCollection<Instrument>> GetAllAsync()
        {
            return _instrumentsAccessService.GetAllAsync();
        }

        public Task<Instrument> GetByAssetPairIdAsync(string assetPair)
        {
            return _instrumentsAccessService.GetByAssetPairIdAsync(assetPair);
        }

        public async Task AddAsync(Instrument instrument)
        {
            await _instrumentsAccessService.AddAsync(instrument);

            if (instrument.State == InstrumentState.Active)
            {
                await _pricesGenerator.Start(instrument.AssetPair);
            }
            else
            {
                await _pricesGenerator.Stop(instrument.AssetPair);
            }
        }

        public async Task UpdateAsync(Instrument instrument)
        {
            var shouldUpdateState = await _instrumentsAccessService.UpdateAsync(instrument);

            if (shouldUpdateState)
            {
                if (instrument.State == InstrumentState.Active)
                {
                    await _pricesGenerator.Start(instrument.AssetPair);
                }
                else
                {
                    await _pricesGenerator.Stop(instrument.AssetPair);
                }
            }
        }

        public Task DeleteAsync(string assetPair)
        {
            return _instrumentsAccessService.DeleteAsync(assetPair);
        }
    }
}
