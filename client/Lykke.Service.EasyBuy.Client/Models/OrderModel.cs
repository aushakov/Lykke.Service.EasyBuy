using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.EasyBuy.Client.Models
{
    /// <summary>
    /// Represents user's order.
    /// </summary>
    [PublicAPI]
    public class OrderModel
    {
        /// <summary>
        /// Order's unique identifier.
        /// </summary>
        public string Id { set; get; }
        
        /// <summary>
        /// Client's wallet Id.
        /// </summary>
        public string WalletId { set; get; }
        
        /// <summary>
        /// Asset pair of the order.
        /// </summary>
        public string AssetPair { set; get; }
        
        /// <summary>
        /// Type of the order.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderType Type { set; get; }
        
        /// <summary>
        /// Id of the calculated price snapshot.
        /// </summary>
        public string PriceId { set; get; }
        
        /// <summary>
        /// Desired volume of the order.
        /// </summary>
        public decimal Volume { set; get; }
        
        /// <summary>
        /// Date and time of order creation.
        /// </summary>
        public DateTime CreatedTime { set; get; }
        
        /// <summary>
        /// Order status.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public OrderStatusModel Status { set; get; }
        
        /// <summary>
        /// Cancellation reason (if applicable).
        /// </summary>
        public string RejectReason { set; get; }
    }
}
