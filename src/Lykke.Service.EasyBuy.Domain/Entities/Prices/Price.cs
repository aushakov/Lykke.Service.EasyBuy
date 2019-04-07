using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Service.EasyBuy.Domain.Entities.OrderBooks;
using Lykke.Service.EasyBuy.Domain.Exceptions;

namespace Lykke.Service.EasyBuy.Domain.Entities.Prices
{
    public class Price
    {
        public string Id { set; get; }

        public string AssetPair { set; get; }

        public OrderType Type { set; get; }

        public decimal Value { set; get; }

        public decimal BaseVolume { set; get; }

        public decimal QuotingVolume { set; get; }

        public decimal Markup { set; get; }

        public decimal OriginalPrice { set; get; }

        public string Exchange { set; get; }

        public DateTime ValidFrom { set; get; }

        public DateTime ValidTo { set; get; }

        public TimeSpan AllowedOverlap { set; get; }

        public static Price Calculate(OrderBook orderBook, decimal quoteVolume, decimal markup, DateTime validFrom,
            DateTime validTo, TimeSpan overlapTime, int priceAccuracy, int volumeAccuracy)
        {
            if (orderBook == null)
                throw new ArgumentNullException(nameof(orderBook));

            if (quoteVolume <= 0)
                throw new FailedOperationException("Quoting volume required");

            if (markup < 0 || 1 < markup)
                throw new FailedOperationException("Invalid markup");

            IReadOnlyList<OrderBookLevel> sellLevels = orderBook.SellLevels;

            if (sellLevels == null || sellLevels.Count == 0)
                throw new FailedOperationException("Empty order book");

            decimal cumulativeVolume = 0;
            decimal oppositeVolume = 0;

            foreach (OrderBookLevel sellLevel in sellLevels.OrderBy(o => o.Price))
            {
                cumulativeVolume += sellLevel.Volume;
                oppositeVolume += sellLevel.Volume * sellLevel.Price;

                if (quoteVolume <= oppositeVolume)
                    break;
            }

            if (oppositeVolume < quoteVolume)
                throw new FailedOperationException("No liquidity");

            decimal originalPrice = (oppositeVolume / cumulativeVolume)
                .TruncateDecimalPlaces(priceAccuracy, true);

            decimal price = (originalPrice * (1 + markup))
                .TruncateDecimalPlaces(priceAccuracy, true);

            decimal volume = Math.Round(quoteVolume / price, volumeAccuracy);

            return new Price
            {
                Id = Guid.NewGuid().ToString(),
                AssetPair = orderBook.AssetPair,
                Type = OrderType.Sell,
                Value = price,
                BaseVolume = volume,
                QuotingVolume = quoteVolume,
                Markup = markup,
                OriginalPrice = originalPrice,
                Exchange = orderBook.Exchange,
                ValidFrom = validFrom,
                ValidTo = validTo,
                AllowedOverlap = overlapTime
            };
        }
    }
}
