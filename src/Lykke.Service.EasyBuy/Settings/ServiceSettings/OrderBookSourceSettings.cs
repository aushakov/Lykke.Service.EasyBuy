using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.EasyBuy.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class OrderBookSourceSettings
    {
        [AmqpCheck]
        public string ConnectionString { set; get; }
        
        public string QueueSuffix { set; get; }
        
        public string[] Exchanges { set; get; }
    }
}
