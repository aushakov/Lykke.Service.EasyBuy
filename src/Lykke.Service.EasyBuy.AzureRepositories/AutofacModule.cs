using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.EasyBuy.AzureRepositories.Instruments;
using Lykke.Service.EasyBuy.AzureRepositories.Orders;
using Lykke.Service.EasyBuy.AzureRepositories.Prices;
using Lykke.Service.EasyBuy.AzureRepositories.Settings;
using Lykke.Service.EasyBuy.AzureRepositories.Trades;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.EasyBuy.AzureRepositories
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<string> _connectionString;

        public AutofacModule(IReloadingManager<string> connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(container => new InstrumentSettingsRepository(
                    AzureTableStorage<InstrumentSettingsEntity>.Create(_connectionString,
                        "Instruments", container.Resolve<ILogFactory>())))
                .As<IInstrumentSettingsRepository>()
                .SingleInstance();

            builder.Register(container => new OrderRepository(
                    AzureTableStorage<OrderEntity>.Create(_connectionString,
                        "Orders", container.Resolve<ILogFactory>()),
                    AzureTableStorage<AzureIndex>.Create(_connectionString,
                        "Orders", container.Resolve<ILogFactory>())))
                .As<IOrderRepository>()
                .SingleInstance();

            builder.Register(container => new PriceRepository(
                    AzureTableStorage<PriceEntity>.Create(_connectionString,
                        "Prices", container.Resolve<ILogFactory>())))
                .As<IPriceRepository>()
                .SingleInstance();

            builder.Register(container => new TimersSettingsRepository(
                    AzureTableStorage<TimersSettingsEntity>.Create(_connectionString,
                        "Settings", container.Resolve<ILogFactory>())))
                .As<ITimersSettingsRepository>()
                .SingleInstance();

            builder.Register(container => new TradeRepository(
                    AzureTableStorage<TradeEntity>.Create(_connectionString,
                        "Trades", container.Resolve<ILogFactory>())))
                .As<ITradeRepository>()
                .SingleInstance();
        }
    }
}
