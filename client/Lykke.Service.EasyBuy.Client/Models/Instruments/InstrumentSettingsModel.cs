using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.EasyBuy.Client.Models.Instruments
{
    /// <summary>
    /// Represents an instrument settings.
    /// </summary>
    [PublicAPI]
    public class InstrumentSettingsModel
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
        [JsonConverter(typeof(StringEnumConverter))]
        public InstrumentStatus Status { get; set; }
    }
}
