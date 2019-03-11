using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.EasyBuy.Domain
{
    public class OrderBook
    {
        public string AssetPair { get; set; }

        public DateTime Timestamp { get; set; }

        public List<OrderBookLimitOrder> SellLimitOrders { get; set; }

        public List<OrderBookLimitOrder> BuyLimitOrders { get; set; }

        public decimal? GetMidPrice()
        {
            var bestSellOrder = SellLimitOrders
                .OrderBy(x => x.Price)
                .FirstOrDefault();
            
            var bestBuyOrder = BuyLimitOrders
                .OrderByDescending(x => x.Price)
                .FirstOrDefault();

            if (bestBuyOrder == null)
            {
                return bestSellOrder?.Price ?? default(decimal);
            }

            if (bestSellOrder == null)
            {
                return bestBuyOrder.Price;
            }
            
            return (bestSellOrder.Price + bestBuyOrder.Price) / 2m;
        }
    }
}
