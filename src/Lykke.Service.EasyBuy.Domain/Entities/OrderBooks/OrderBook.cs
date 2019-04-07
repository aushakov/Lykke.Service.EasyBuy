using System;
using System.Collections.Generic;

namespace Lykke.Service.EasyBuy.Domain.Entities.OrderBooks
{
    public class OrderBook
    {
        public string Exchange { get; set; }

        public string AssetPair { get; set; }

        public DateTime Timestamp { get; set; }

        public IReadOnlyList<OrderBookLevel> SellLevels { get; set; }

        public IReadOnlyList<OrderBookLevel> BuyLevels { get; set; }
    }
}
