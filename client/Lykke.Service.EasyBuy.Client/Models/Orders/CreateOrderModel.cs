using JetBrains.Annotations;

namespace Lykke.Service.EasyBuy.Client.Models.Orders
{
    /// <summary>
    /// Used to request order creation.
    /// </summary>
    [PublicAPI]
    public class CreateOrderModel
    {
        /// <summary>
        /// Client's wallet Id.
        /// </summary>
        public string WalletId;

        /// <summary>
        /// Price Id that should be used.
        /// </summary>
        public string PriceId;

        /// <summary>
        /// Desired quoting volume.
        /// </summary>
        public decimal QuotingVolume;
    }
}
