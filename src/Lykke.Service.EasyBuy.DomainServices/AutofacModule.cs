using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly string _instanceName;
        private readonly string _walletId;
        private readonly TimeSpan _recalculationInterval;

        public AutofacModule(
            string instanceName,
            string walletId,
            TimeSpan recalculationInterval)
        {
            _instanceName = instanceName;
            _walletId = walletId;
            _recalculationInterval = recalculationInterval;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<BalanceService>()
                .As<IBalancesService>()
                .SingleInstance();

            builder.RegisterType<InstrumentSettingsService>()
                .As<IInstrumentSettingsService>()
                .SingleInstance();

            builder.RegisterType<InternalTransfersService>()
                .As<IInternalTransfersService>()
                .SingleInstance();

            builder.RegisterType<OrderBookService>()
                .As<IOrderBookService>()
                .SingleInstance();

            builder.RegisterType<OrdersService>()
                .As<IOrdersService>()
                .SingleInstance();

            builder.RegisterType<PriceService>()
                .As<IPriceService>()
                .SingleInstance();

            builder.RegisterType<SettingsService>()
                .WithParameter("instanceName", _instanceName)
                .WithParameter("walletId", _walletId)
                .WithParameter("recalculationInterval", _recalculationInterval)
                .As<ISettingsService>()
                .SingleInstance();
            
            builder.RegisterType<TradeService>()
                .As<ITradeService>()
                .SingleInstance();
        }
    }
}
