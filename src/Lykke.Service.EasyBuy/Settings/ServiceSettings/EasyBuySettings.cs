using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Settings.ServiceSettings.Db;

namespace Lykke.Service.EasyBuy.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class EasyBuySettings
    {
        public DbSettings Db { get; set; }
    }
}
