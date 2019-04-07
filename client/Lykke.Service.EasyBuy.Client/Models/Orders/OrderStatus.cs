using JetBrains.Annotations;

namespace Lykke.Service.EasyBuy.Client.Models.Orders
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public enum OrderStatus
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        
        /// <summary>
        /// 
        /// </summary>
        New,
        
        /// <summary>
        /// 
        /// </summary>
        Cancelled,
        
        /// <summary>
        /// 
        /// </summary>
        Reserved,
        
        /// <summary>
        /// 
        /// </summary>
        Completed
    }
}
