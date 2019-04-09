using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Client.Models;
using Lykke.Service.EasyBuy.Client.Models.Orders;
using Refit;

namespace Lykke.Service.EasyBuy.Client.Api
{
    /// <summary>
    /// Provides methods to work with orders.
    /// </summary>
    public interface IOrdersApi
    {
        /// <summary>
        /// Used to create order.
        /// </summary>
        /// <returns>Order details.</returns>
        [Post("/api/orders")]
        Task<OrderModel> CreateOrderAsync(CreateOrderModel model);

        /// <summary>
        /// Used to fetch existing order.
        /// </summary>
        /// <param name="orderId">Id of the order.</param>
        /// <returns>Order details.</returns>
        [Get("/api/orders/{orderId}")]
        Task<OrderModel> GetOrderByIdAsync(string orderId);

        /// <summary>
        /// Gets all orders with filters. Filter isn't applied if equal tu null.
        /// </summary>
        /// <param name="walletId">Client's wallet Id.</param>
        /// <param name="assetPair">Asset pair of the order.</param>
        /// <param name="timeFrom">Time from which the orders should be filtered (inclusive).</param>
        /// <param name="timeTo">Time up to which the orders should be filtered (inclusive).</param>
        /// <param name="limit">How many records to take.</param>
        /// <returns></returns>
        [Get("/api/orders")]
        Task<IReadOnlyList<OrderModel>> GetAllOrdersAsync(
            string walletId,
            string assetPair,
            DateTime? timeFrom,
            DateTime? timeTo,
            int limit);
    }
}
