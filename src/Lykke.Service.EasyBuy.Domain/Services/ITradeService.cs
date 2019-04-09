using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Trades;

namespace Lykke.Service.EasyBuy.Domain.Services
{
    public interface ITradeService
    {
        Task<IReadOnlyList<Trade>> GetAsync(DateTime from, DateTime to);

        Task AddAsync(Trade trade);
    }
}
