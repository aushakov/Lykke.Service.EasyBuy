using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Orders;

namespace Lykke.Service.EasyBuy.Domain.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(string orderId);

        Task<IReadOnlyList<Order>> GetAllAsync(string walletId, string assetPair, DateTime? timeFrom, DateTime? timeTo,
            int limit);

        Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status);

        Task InsertAsync(Order order);

        Task UpdateAsync(Order order);
    }
}
