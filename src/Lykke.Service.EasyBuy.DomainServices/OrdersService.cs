using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client.Models.v3;
using Lykke.Service.Assets.Client.ReadModels;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Entities.Instruments;
using Lykke.Service.EasyBuy.Domain.Entities.Orders;
using Lykke.Service.EasyBuy.Domain.Entities.Prices;
using Lykke.Service.EasyBuy.Domain.Entities.Trades;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class OrdersService : IOrdersService
    {
        private readonly IPriceService _priceService;
        private readonly IInstrumentSettingsService _instrumentSettingsService;
        private readonly ISettingsService _settingsService;
        private readonly IInternalTransfersService _internalTransfersService;
        private readonly IAssetsReadModelRepository _assetsRepository;
        private readonly IAssetPairsReadModelRepository _assetPairsRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITradeService _tradeService;
        private readonly ILog _log;

        public OrdersService(
            IPriceService priceService,
            IInstrumentSettingsService instrumentSettingsService,
            IInternalTransfersService internalTransfersService,
            IAssetsReadModelRepository assetsRepository,
            IAssetPairsReadModelRepository assetPairsRepository,
            IOrderRepository orderRepository,
            ISettingsService settingsService,
            ITradeService tradeService,
            ILogFactory logFactory)
        {
            _priceService = priceService;
            _instrumentSettingsService = instrumentSettingsService;
            _internalTransfersService = internalTransfersService;
            _assetsRepository = assetsRepository;
            _assetPairsRepository = assetPairsRepository;
            _orderRepository = orderRepository;
            _settingsService = settingsService;
            _tradeService = tradeService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<Order> CreateAsync(string walletId, string priceId, decimal quotingVolume)
        {
            Price price = await _priceService.GetByIdAsync(priceId);

            if (price == null)
                throw new EntityNotFoundException();

            InstrumentSettings instrumentSettings =
                await _instrumentSettingsService.GetByAssetPairAsync(price.AssetPair);

            if (instrumentSettings.Status != InstrumentStatus.Active)
                throw new FailedOperationException($"Instrument {instrumentSettings.AssetPair} isn't active.");

            if (price.ValidTo + price.AllowedOverlap < DateTime.UtcNow)
                throw new FailedOperationException("Given price too old.");

            if (price.QuotingVolume < quotingVolume)
                throw new FailedOperationException("Requested volume higher than initial.");

            AssetPair pair = _assetPairsRepository.TryGet(price.AssetPair);

            Asset baseAsset = _assetsRepository.TryGet(pair.BaseAssetId);

            decimal baseVolume = quotingVolume == price.QuotingVolume
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

            await _orderRepository.InsertAsync(order);

            _log.Info("Order created.", order);

            try
            {
                await _internalTransfersService.TransferAsync(
                    order.ReserveTransferId,
                    walletId,
                    _settingsService.GetWalletId(),
                    order.Type == OrderType.Buy
                        ? pair.QuotingAssetId
                        : pair.BaseAssetId,
                    order.Type == OrderType.Buy
                        ? quotingVolume
                        : baseVolume);
            }
            catch (MeNotEnoughFundsException)
            {
                var rejectReason = "Client doesn't have enough funds.";

                await PersistWithStatusAsync(order, OrderStatus.Cancelled, rejectReason);

                throw new FailedOperationException(rejectReason);
            }
            catch (MeOperationException e)
            {
                var rejectReason = "ME call failed.";

                await PersistWithStatusAsync(order, OrderStatus.Cancelled, rejectReason);

                _log.Warning(rejectReason, priceId, e);

                throw new FailedOperationException(rejectReason);
            }
            catch (FailedOperationException e)
            {
                var rejectReason = "ME call failed.";

                await PersistWithStatusAsync(order, OrderStatus.Cancelled, rejectReason);

                _log.Warning(rejectReason, priceId, e);

                throw;
            }

            await PersistWithStatusAsync(order, OrderStatus.Reserved);

            return order;
        }

        public async Task ProcessPendingAsync()
        {
            var executedOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Reserved);

            foreach (var order in executedOrders)
            {
                var easyBuyWalletId = _settingsService.GetWalletId();

                var pair = _assetPairsRepository.TryGet(order.AssetPair);

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

                    await _tradeService.AddAsync(new Trade
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
            var order = await _orderRepository.GetAsync(walletId, id);

            if (order == null)
                throw new EntityNotFoundException();

            return order;
        }

        public Task<IReadOnlyList<Order>> GetAllAsync(string walletId, string assetPair, DateTime? timeFrom,
            DateTime? timeTo, int limit)
        {
            return _orderRepository.GetAllAsync(walletId, assetPair, timeFrom, timeTo, limit);
        }

        private Task PersistWithStatusAsync(Order order, OrderStatus status, string rejectReason = null)
        {
            order.Status = status;

            if (status == OrderStatus.Cancelled)
            {
                if (string.IsNullOrWhiteSpace(rejectReason))
                    throw new Exception("Provide reject reason when cancelling order.");

                order.RejectReason = rejectReason;
            }

            return _orderRepository.UpdateAsync(order);
        }
    }
}
