using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.EasyBuy.AzureRepositories.DefaultSettings;
using Lykke.Service.EasyBuy.AzureRepositories.Instruments;
using Lykke.Service.EasyBuy.AzureRepositories.Orders;
using Lykke.Service.EasyBuy.AzureRepositories.Prices;
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
            builder.Register(container => new InstrumentsRepository(
                    AzureTableStorage<InstrumentEntity>.Create(_connectionString,
                        "Instruments", container.Resolve<ILogFactory>())))
                .As<IInstrumentsRepository>()
                .SingleInstance();

            builder.Register(container => new PricesRepository(
                    AzureTableStorage<PriceEntity>.Create(_connectionString,
                        "Prices", container.Resolve<ILogFactory>())))
                .As<IPricesRepository>()
                .SingleInstance();

            builder.Register(container => new OrdersRepository(
                    AzureTableStorage<OrderEntity>.Create(_connectionString,
                        "Orders", container.Resolve<ILogFactory>()),
                    AzureTableStorage<AzureIndex>.Create(_connectionString,
                        "Orders", container.Resolve<ILogFactory>())))
                .As<IOrdersRepository>()
                .SingleInstance();

            builder.Register(container => new TradesRepository(
                    AzureTableStorage<TradeEntity>.Create(_connectionString,
                        "Trades", container.Resolve<ILogFactory>())))
                .As<ITradesRepository>()
                .SingleInstance();

            builder.Register(container => new DefaultSettingsRepository(
                    AzureTableStorage<DefaultSettingsEntity>.Create(_connectionString,
                        "DefaultSettings", container.Resolve<ILogFactory>())))
                .As<IDefaultSettingsRepository>()
                .SingleInstance();
        }
    }
}
