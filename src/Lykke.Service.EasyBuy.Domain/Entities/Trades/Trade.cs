using System;

namespace Lykke.Service.EasyBuy.Domain.Entities.Trades
{
    public class Trade
    {
        public string Id { set; get; }

        public string WalletId { set; get; }

        public string OrderId { set; get; }

        public OrderType Type { set; get; }

        public decimal BaseVolume { set; get; }

        public decimal QuotingVolume { set; get; }

        public DateTime CreationDateTime { set; get; }
    }
}
