using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Settings.ServiceSettings.Db;

namespace Lykke.Service.EasyBuy.Settings.ServiceSettings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class EasyBuySettings
    {
        public string InstanceName { set; get; }
        
        public string WalletId { set; get; }
        
        public DbSettings Db { get; set; }
        
        public RabbitPublishSettings PricesPublish { set; get; }
        
        public OrderBookSourceSettings OrderBookSource { set; get; }
    }
}
