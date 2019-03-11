using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class OrdersService : IOrdersService
    {
        private readonly IAssetsServiceWithCache _assetsService;
        private readonly IInstrumentsAccessService _instrumentsAccessService;
        private readonly ISettingsService _settingsService;
        private readonly IInternalTransfersService _internalTransfersService;
        private readonly IPricesService _pricesService;
        private readonly IOrdersRepository _ordersRepository;
        private readonly ITradesRepository _tradesRepository;
        private readonly ILog _log;

        public OrdersService(
            IPricesService pricesService,
            IInstrumentsAccessService instrumentsAccessService,
            IInternalTransfersService internalTransfersService,
            IAssetsServiceWithCache assetsService,
            IOrdersRepository ordersRepository,
            ISettingsService settingsService,
            ITradesRepository tradesRepository,
            ILogFactory logFactory)
        {
            _pricesService = pricesService;
            _internalTransfersService = internalTransfersService;
            _assetsService = assetsService;
            _ordersRepository = ordersRepository;
            _settingsService = settingsService;
            _tradesRepository = tradesRepository;
            _instrumentsAccessService = instrumentsAccessService;
            _log = logFactory.CreateLog(this);
        }
        
        public async Task<Order> CreateAsync(string walletId, string priceId, decimal quotingVolume)
        {
            
            var price = await _pricesService.GetAsync(priceId);
            
            if(price == null)
                throw new EntityNotFoundException();

            var instrument = await _instrumentsAccessService.GetByAssetPairIdAsync(price.AssetPair);
            
            if(instrument.State != InstrumentState.Active)
                throw new FailedOperationException($"Instrument {instrument.AssetPair} isn't active.");

            var defaultSettings = await _settingsService.GetDefaultSettingsAsync();
            
            if(DateTime.UtcNow > price.ValidTo + (instrument.OverlapTime ?? defaultSettings.OverlapTime))
                throw new FailedOperationException("Given price too old.");
            
            if(quotingVolume > price.QuotingVolume)
                throw new FailedOperationException("Requested volume higher than initial.");

            var pair = await _assetsService.TryGetAssetPairAsync(price.AssetPair);

            var baseAsset = await _assetsService.TryGetAssetAsync(pair.BaseAssetId);

            var baseVolume = quotingVolume == price.QuotingVolume
                ? price.BaseVolume
                : (quotingVolume / price.Value).TruncateDecimalPlaces(baseAsset.Accuracy, price.Type == OrderType.Sell);
            
            var order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                WalletId = walletId,
                Type = price.Type,
                AssetPair = price.AssetPair,
                QuotingVolume = quotingVolume,
                BaseVolume = baseVolume,
                PriceId = priceId,
                CreatedTime = DateTime.UtcNow,
                Status = OrderStatus.New,
                ReserveTransferId = Guid.NewGuid().ToString(),
                SettlementTransferId = Guid.NewGuid().ToString()
            };

            await _ordersRepository.InsertAsync(order);
            
            _log.Info("Order was created.", order);

            try
            {
                await _internalTransfersService.TransferAsync(
                    order.ReserveTransferId,
                    walletId,
                    await _settingsService.GetWalletIdAsync(),
                    order.Type == OrderType.Buy
                        ? pair.QuotingAssetId
                        : pair.BaseAssetId,
                    order.Type == OrderType.Buy
                        ? quotingVolume
                        : baseVolume);

            }
            catch (MeNotEnoughFundsException)
            {
                await PersistWithStatusAsync(order, OrderStatus.Cancelled);

                throw new FailedOperationException("Client doesn't have enough funds.");
            }
            catch (MeOperationException e)
            {
                await PersistWithStatusAsync(order, OrderStatus.Cancelled);

                _log.Warning("ME call failed.", priceId, e);

                throw new FailedOperationException("ME call failed.");
            }
            catch (FailedOperationException e)
            {
                await PersistWithStatusAsync(order, OrderStatus.Cancelled);

                _log.Warning("ME call failed.", priceId, e);
                
                throw;
            }

            await PersistWithStatusAsync(order, OrderStatus.Reserved);

            return order;
        }
        
        public async Task ProcessPendingAsync()
        {
            var executedOrders = await _ordersRepository.GetByStatusAsync(OrderStatus.Reserved);

            foreach (var order in executedOrders)
            {
                var easyBuyWalletId = await _settingsService.GetWalletIdAsync();
                
                var pair = await _assetsService.TryGetAssetPairAsync(order.AssetPair);

                try
                {
                    await _internalTransfersService.TransferAsync(
                        order.SettlementTransferId,
                        easyBuyWalletId,
                        order.WalletId,
                        order.Type == OrderType.Buy
                            ? pair.BaseAssetId
                            : pair.QuotingAssetId,
                        order.Type == OrderType.Buy
                            ? order.BaseVolume
                            : order.QuotingVolume);

                    await PersistWithStatusAsync(order, OrderStatus.Completed);

                    await _tradesRepository.InsertAsync(new Trade
                    {
                        Id = Guid.NewGuid().ToString(),
                        BaseVolume = order.BaseVolume,
                        QuotingVolume = order.QuotingVolume,
                        WalletId = order.WalletId,
                        OrderId = order.Id,
                        Type = order.Type,
                        CreationDateTime = DateTime.UtcNow
                    });
                }
                catch (Exception e)
                {
                    _log.Error(e);
                }
            }
        }

        public async Task<Order> GetAsync(string walletId, string id)
        {
            var order = await _ordersRepository.GetAsync(walletId, id);

            if (order == null)
                throw new EntityNotFoundException();

            return order;
        }
        
        private Task PersistWithStatusAsync(Order order, OrderStatus status)
        {
            order.Status = status;
            
            return _ordersRepository.UpdateAsync(order);
        }
    }
}
