using System;

namespace Lykke.Service.EasyBuy.Domain.Entities.Instruments
{
    /// <summary>
    /// Represents an instrument settings.
    /// </summary>
    public class InstrumentSettings
    {
        /// <summary>
        /// The name of asset pair.
        /// </summary>
        public string AssetPair { set; get; }

        /// <summary>
        /// The name of the exchange.
        /// </summary>
        public string Exchange { set; get; }

        /// <summary>
        /// The time interval while the price is available to create orders.
        /// </summary>
        public TimeSpan PriceLifetime { set; get; }

        /// <summary>
        /// The time interval since price live time expired but the price continues to be available.
        /// </summary>
        public TimeSpan OverlapTime { set; get; }

        /// <summary>
        /// The markup that applied to the original price.
        /// </summary>
        public decimal Markup { set; get; }

        /// <summary>
        /// The quoting volume that used to calculate client price and volume.
        /// </summary>
        public decimal Volume { set; get; }

        /// <summary>
        /// Indicated the current status of instrument.
        /// </summary>
        public InstrumentStatus Status { get; set; }

        public void Update(InstrumentSettings instrumentSettings)
        {
            AssetPair = instrumentSettings.AssetPair;
            Exchange = instrumentSettings.Exchange;
            PriceLifetime = instrumentSettings.PriceLifetime;
            OverlapTime = instrumentSettings.OverlapTime;
            Markup = instrumentSettings.Markup;
            Volume = instrumentSettings.Volume;
        }
    }
}
