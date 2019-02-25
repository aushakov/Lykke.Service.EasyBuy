using Autofac;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.EasyBuy
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public AutofacModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new DomainServices.AutofacModule());
            builder.RegisterModule(new AzureRepositories.AutofacModule(_settings.Nested(o =>
                o.EasyBuyService.Db.DataConnectionString)));
        }
    }
}
