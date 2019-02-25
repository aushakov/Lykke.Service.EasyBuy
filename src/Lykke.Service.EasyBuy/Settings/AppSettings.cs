using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.EasyBuy.Settings.ServiceSettings;

namespace Lykke.Service.EasyBuy.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public EasyBuySettings EasyBuyService { get; set; }
    }
}
