using System;

namespace Lykke.Service.EasyBuy.Domain
{
    public class DefaultSetting
    {
        public decimal Markup { set; get; }
        
        public TimeSpan RecalculationInterval { set; get; }
        
        public TimeSpan OverlapTime { set; get; }
        
        public TimeSpan PriceLifetime { set; get; }
        
        public TimeSpan TimerPeriod { set; get; }
    }
}
