using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.EasyBuy.Contract
{
    /// <summary>
    /// Represents a price update.
    /// </summary>
    [PublicAPI]
    public class Price
    {
        /// <summary>
        /// Price's unique identifier.
        /// </summary>
        public string Id { set; get; }
        
        /// <summary>
        /// Asset pair for which the price is calculated.
        /// </summary>
        public string AssetPair { set; get; }
        
        /// <summary>
        /// Direction in which client wishes to perform an operation.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType Type { set; get; }
        
        /// <summary>
        /// Possible base volume of the operation.
        /// </summary>
        public decimal BaseVolume { set; get; }
        
        /// <summary>
        /// Desired quoting volume of the operation.
        /// </summary>
        public decimal QuotingVolume { set; get; }
        
        /// <summary>
        /// Total calculated price.
        /// </summary>
        public decimal Value { set; get; }
        
        /// <summary>
        /// When the price becomes valid.
        /// </summary>
        public DateTime ValidFrom { set; get; }
        
        /// <summary>
        /// When the price stops being valid.
        /// </summary>
        public DateTime ValidTo { set; get; }
    }
}
