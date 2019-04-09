using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Orders;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface IOrdersService
    {
        Task<Order> CreateAsync(string walletId, string priceId, decimal quotingVolume);

        Task ProcessPendingAsync();

        Task<Order> GetAsync(string walletId, string id);
        
        Task<IReadOnlyList<Order>> GetAllAsync(string walletId, string assetPair, DateTime? timeFrom, DateTime? timeTo, int limit);
    }
}
