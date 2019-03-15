using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using JetBrains.Annotations;
using Lykke.Service.Assets.Client;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class PricesService : IPricesService
    {
        private readonly IAssetsServiceWithCache _assetsService;
        private readonly IInstrumentsAccessService _instrumentsAccessService;
        private readonly IOrderBookService _orderBookService;
        private readonly IPricesRepository _pricesRepository;
        private readonly ISettingsService _settingsService;
        
        private readonly ConcurrentDictionary<string, Price> _latestPrices;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores;
        private readonly object _semaphoresLock;
        
        public PricesService(
            IAssetsServiceWithCache assetsService,
            IInstrumentsAccessService instrumentsAccessService,
            IOrderBookService orderBookService,
            IPricesRepository pricesRepository,
            ISettingsService settingsService)
        {
            _assetsService = assetsService;
            _instrumentsAccessService = instrumentsAccessService;
            _orderBookService = orderBookService;
            _pricesRepository = pricesRepository;
            _settingsService = settingsService;
            
            _latestPrices = new ConcurrentDictionary<string, Price>();
            _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
            _semaphoresLock = new object();
        }
        
        public async Task<Price> CreateAsync(string assetPair, OrderType type, decimal quotingVolume, DateTime validFrom)
        {
            var instrument = await _instrumentsAccessService.GetByAssetPairIdAsync(assetPair);

            if (instrument == null || instrument.State != InstrumentState.Active)
            {
                throw new FailedOperationException($"No active instrument {assetPair} was found.");
            }

            var orderBook = _orderBookService.GetByAssetPairId(instrument.Exchange, assetPair);

            if(type == OrderType.None)
                throw new FailedOperationException($"Invalid order type {OrderType.None}");

            var pair = await _assetsService.TryGetAssetPairAsync(assetPair);
            
            if(pair == null)
                throw new FailedOperationException($"Pair {assetPair} not found.");

            var baseAsset = await _assetsService.TryGetAssetAsync(pair.BaseAssetId);
            var quotingAsset = await _assetsService.TryGetAssetAsync(pair.QuotingAssetId);
            
            if(baseAsset == null)
                throw new FailedOperationException($"Base asset {pair.BaseAssetId} not found.");
            
            if(quotingAsset == null)
                throw new FailedOperationException($"Quoting asset {pair.QuotingAssetId} not found.");

            var orders = type == OrderType.Buy
                ? orderBook.SellLimitOrders.OrderBy(x => x.Price).ToList()
                : orderBook.BuyLimitOrders.OrderByDescending(x => x.Price).ToList();
            
            if (!orders.Any() || orders.Sum(x => x.Volume * x.Price) < quotingVolume)
            {
                throw new FailedOperationException("Not enough liquidity.");
            }

            var defaultSettings = await _settingsService.GetDefaultSettingsAsync();

            var markupCoefficient = instrument.Markup ?? defaultSettings.Markup;

            var midPrice = orderBook.GetMidPrice();
            
            if(!midPrice.HasValue)
            {
                throw new FailedOperationException("Not enough liquidity.");
            }
            
            var markupAbsolute = midPrice.Value * markupCoefficient;
            
            var volumes = new List<decimal>();
            var remainingVolume = quotingVolume;

            for (var i = 0; i < orders.Count; i++)
            {
                var orderVolumeInUsd = orders[i].Volume * orders[i].Price;
                    
                if (remainingVolume == 0m || remainingVolume <= (decimal) pair.MinVolume)
                    break;

                if (orderVolumeInUsd <= remainingVolume)
                {
                    volumes.Add(orders[i].Volume);

                    remainingVolume -= orderVolumeInUsd;
                }
                else
                {
                    volumes.Add(remainingVolume / orders[i].Price);

                    break;
                }
            }

            var volumeBase = Math.Round(volumes.Sum(), baseAsset.Accuracy);

            var originalPrice = (quotingVolume / volumes.Sum()).TruncateDecimalPlaces(pair.Accuracy, type == OrderType.Buy);

            var overallPrice =
                (originalPrice + markupAbsolute * (type == OrderType.Buy ? 1m : -1m))
                    .TruncateDecimalPlaces(pair.Accuracy, type == OrderType.Buy);
            
            var validTo = validFrom + (instrument.PriceLifetime ?? defaultSettings.PriceLifetime);

            var allowedOverlap = instrument.OverlapTime ?? defaultSettings.OverlapTime;

            var price = new Price
            {
                Id = Guid.NewGuid().ToString(),
                Value = overallPrice,
                AssetPair = assetPair,
                Exchange = instrument.Exchange,
                BaseVolume = volumeBase,
                QuotingVolume = quotingVolume,
                Markup = markupCoefficient,
                OriginalPrice = originalPrice,
                Type = type,
                ValidFrom = validFrom,
                ValidTo = validTo,
                AllowedOverlap = allowedOverlap
            };

            await _pricesRepository.InsertAsync(price);

            await UpdateIfLatestAsync(price);
            
            return price;
        }

        public async Task<IReadOnlyList<Price>> GetActiveAsync(OrderType type)
        {
            var activeInstruments = (await _instrumentsAccessService.GetAllAsync())
                .Where(x => x.State == InstrumentState.Active);
            
            var prices = new List<Price>();

            foreach (var instrument in activeInstruments)
            {
                var lastPrice = await GetLatestAsync(instrument.AssetPair, type);
                
                if(lastPrice != null)
                    prices.Add(lastPrice);
            }

            return prices;
        }

        public async Task<Price> GetAsync(string id)
        {
            var priceSnapshot = await _pricesRepository.GetAsync(id);
            
            if (priceSnapshot == null)
                throw new EntityNotFoundException();

            return priceSnapshot;
        }

        private async Task UpdateIfLatestAsync(Price price)
        {
            var semaphore = ObtainSemaphore(price.AssetPair, price.Type);

            await semaphore.WaitAsync();

            try
            {
                var key = GetLastPriceKey(price.AssetPair, price.Type);

                if (!_latestPrices.ContainsKey(key))
                {
                    _latestPrices[key] = price;
                    return;
                }

                var latestPriceInDictionary = _latestPrices[key];

                if (latestPriceInDictionary.ValidTo >= price.ValidTo)
                    return;

                _latestPrices[key] = price;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private async Task<Price> GetLatestAsync(string assetPair, OrderType type)
        {
            var semaphore = ObtainSemaphore(assetPair, type);

            await semaphore.WaitAsync();

            try
            {
                var key = GetLastPriceKey(assetPair, type);

                if (_latestPrices.ContainsKey(key))
                    return _latestPrices[key];
                
                var latestPriceFromRepository = await _pricesRepository.GetLatestAsync(assetPair, type);

                if (latestPriceFromRepository == null)
                    return null;

                _latestPrices[key] = latestPriceFromRepository;

                return latestPriceFromRepository;
            }
            finally
            {
                semaphore.Release();
            }
        }

        private SemaphoreSlim ObtainSemaphore(string assetPair, OrderType type)
        {
            lock (_semaphoresLock)
            {
                var semaphoreWasPresent = _semaphores.TryGetValue(GetLastPriceKey(assetPair, type), out var semaphore);

                if (semaphoreWasPresent)
                    return semaphore;
                
                semaphore = new SemaphoreSlim(1, 1);

                _semaphores[GetLastPriceKey(assetPair, type)] = semaphore;

                return semaphore;
            }
        }

        private static string GetLastPriceKey(string assetPair, OrderType type)
        {
            return $"{assetPair}_{type.ToString()}";
        }
    }
}
