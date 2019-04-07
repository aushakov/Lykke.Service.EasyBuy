using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.Timers
{
    public class PricesTimer : Timer
    {
        private readonly IPriceService _priceService;
        private readonly ISettingsService _settingsService;
        private readonly IInstrumentSettingsService _instrumentSettingsService;

        public PricesTimer(
            IPriceService priceService,
            ISettingsService settingsService,
            IInstrumentSettingsService instrumentSettingsService,
            ILogFactory logFactory)
        {
            _priceService = priceService;
            _settingsService = settingsService;
            _instrumentSettingsService = instrumentSettingsService;

            Log = logFactory.CreateLog(this);
        }

        protected override async Task OnExecuteAsync(CancellationToken cancellation)
        {
            IReadOnlyList<InstrumentSettings> instrumentsSettings = await _instrumentSettingsService.GetActiveAsync();

            DateTime currentDate = DateTime.UtcNow;

            IEnumerable<Task> tasks = instrumentsSettings
                .Select(o => Task
                    .Run(async () => { await _priceService.EnsureAsync(o.AssetPair, currentDate); }, cancellation)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                            Log.Error(task.Exception, "Something went wrong in calculation and publishing.");
                    }, cancellation));

            await Task.WhenAll(tasks);
        }

        protected override Task<TimeSpan> GetDelayAsync()
        {
            TimeSpan recalculationInterval = _settingsService.GetRecalculationInterval();

            return Task.FromResult(recalculationInterval);
        }
    }
}
