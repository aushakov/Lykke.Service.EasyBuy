using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class PricesGenerator : IPricesGenerator
    {
        private readonly IInstrumentsAccessService _instrumentsAccessService;
        private readonly IPricesService _pricesService;
        private readonly IPricesPublisher _pricesPublisher;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        private readonly ConcurrentDictionary<string, CancellationTokenSource> _tokenSources;
        private readonly ConcurrentDictionary<string, Task> _cycleTasks;
        private readonly SemaphoreSlim _startLock;

        public PricesGenerator(
            IInstrumentsAccessService instrumentsAccessService,
            IPricesPublisher pricesPublisher,
            IPricesService pricesService,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _instrumentsAccessService = instrumentsAccessService;
            _pricesPublisher = pricesPublisher;
            _pricesService = pricesService;
            _settingsService = settingsService;
            _log = logFactory.CreateLog(this);

            _tokenSources = new ConcurrentDictionary<string, CancellationTokenSource>();
            _cycleTasks = new ConcurrentDictionary<string, Task>();
            _startLock = new SemaphoreSlim(1, 1);
        }

        public async Task StartActives()
        {
            foreach (var activeInstrument in (await _instrumentsAccessService.GetAllAsync()).Where(x =>
                x.State == InstrumentState.Active))
            {
                await Start(activeInstrument.AssetPair);
            }
        }

        public async Task StopAll()
        {
            foreach (var assetPair in _cycleTasks.Keys)
            {
                await Stop(assetPair);
            }
        }

        public async Task Start(string assetPair)
        {
            try
            {
                await _startLock.WaitAsync();

                _log.Info(nameof(Start), "Starting publishing.", assetPair);

                var instrument = (await _instrumentsAccessService.GetAllAsync())
                    .SingleOrDefault(x => x.AssetPair == assetPair);

                if (instrument == null || instrument.State != InstrumentState.Active)
                    throw new FailedOperationException($"No active instrument {assetPair} was found.");

                if (_tokenSources.ContainsKey(assetPair))
                    throw new FailedOperationException($"Instrument {assetPair} already running.");

                _tokenSources[assetPair] = new CancellationTokenSource();

                var cycleTask = Task.Run(async () => { await HandleGenerationCycleAsync(instrument.AssetPair); })
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                            _log.Error(t.Exception, "Something went wrong in calculation and publishing thread.");
                    });

                _cycleTasks[assetPair] = cycleTask;

                _log.Info(nameof(Start), "Started.", assetPair);
            }
            finally
            {
                _startLock.Release();
            }
        }

        public async Task Stop(string assetPair)
        {
            try
            {
                await _startLock.WaitAsync();

                _log.Info(nameof(Stop), "Stopping publishing.", assetPair);

                if (!_cycleTasks.ContainsKey(assetPair) || !_tokenSources.ContainsKey(assetPair))
                    throw new FailedOperationException($"No instrument {assetPair} was found running.");

                _tokenSources[assetPair].Cancel();

                await _cycleTasks[assetPair];

                _tokenSources.TryRemove(assetPair, out _);
                _cycleTasks.TryRemove(assetPair, out _);

                _log.Info(nameof(Stop), "Stopped.", assetPair);
            }
            finally
            {
                _startLock.Release();
            }
        }

        private async Task HandleGenerationCycleAsync(string assetPair)
        {
            var lastCalculationTime = DateTime.UtcNow;

            var instrument = await _instrumentsAccessService.GetByAssetPairIdAsync(assetPair);

            var nextPack = await TryToCalculateNext(instrument.AssetPair, instrument.Volume, lastCalculationTime);
                    
            var defaultSettings = await _settingsService.GetDefaultSettingsAsync();

            var lastPriceLifetime = instrument.PriceLifetime ?? defaultSettings.PriceLifetime;
            
            var recalculationThreshold = instrument.RecalculationInterval ?? defaultSettings.RecalculationInterval;

            while (_tokenSources[instrument.AssetPair] != null &&
                   !_tokenSources[instrument.AssetPair].IsCancellationRequested)
            {
                try
                {
                    instrument = await _instrumentsAccessService.GetByAssetPairIdAsync(assetPair);

                    defaultSettings = await _settingsService.GetDefaultSettingsAsync();

                    var priceLifetime = instrument.PriceLifetime ?? defaultSettings.PriceLifetime;

                    lastPriceLifetime = priceLifetime;

                    TryToPublish(instrument.AssetPair, nextPack);

                    await Task.Delay(
                        Min(priceLifetime - recalculationThreshold,
                            lastCalculationTime + priceLifetime - DateTime.UtcNow),
                        _tokenSources[instrument.AssetPair].Token);

                    recalculationThreshold = instrument.RecalculationInterval ?? defaultSettings.RecalculationInterval;

                    nextPack = await TryToCalculateNext(instrument.AssetPair, instrument.Volume,
                        lastCalculationTime + lastPriceLifetime);

                    await Task.Delay(
                        Max(lastCalculationTime + priceLifetime - DateTime.UtcNow, TimeSpan.Zero),
                        _tokenSources[instrument.AssetPair].Token);

                    lastCalculationTime += priceLifetime;
                }
                catch (TaskCanceledException)
                {
                    
                }
                catch (Exception e)
                {
                    _log.Error(e, e.Message, instrument.AssetPair);
                }
            }
        }

        private void TryToPublish(string assetPair, PricesPack pack)
        {
            try
            {
                if (pack != null)
                {
                    _pricesPublisher.Publish(pack.Sell);
                    //_pricesPublisher.Publish(_nextPriceToPublish[assetPair].Buy);
                }
                else
                {
                    _log.Info(nameof(TryToPublish), "Skipping publishing.", assetPair);
                }
            }
            catch (Exception e)
            {
                _log.Warning("Exception while trying to publish.", e, assetPair);
            }
        }

        private async Task<PricesPack> TryToCalculateNext(string assetPair, decimal volume, DateTime validFrom)
        {
            try
            {
                _log.Info(nameof(TryToCalculateNext), "Calculating next pack.", new
                {
                    assetPair,
                    volume,
                    validFrom
                });
                
                return new PricesPack
                {
                    Buy = await _pricesService.CreateAsync(assetPair, OrderType.Buy,
                        volume, validFrom),
                    Sell = await _pricesService.CreateAsync(assetPair, OrderType.Sell,
                        volume, validFrom)
                };
            }
            catch (Exception e)
            {
                _log.Warning("Exception while trying to calculate next price.", e, assetPair);
                return null;
            }
        }

        private static TimeSpan Max(TimeSpan first, TimeSpan second)
        {
            return first > second ? first : second;
        }

        private static TimeSpan Min(TimeSpan first, TimeSpan second)
        {
            return first < second ? first : second;
        }
    }
}
