using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Trades;

namespace Lykke.Service.EasyBuy.Domain.Repositories
{
    public interface ITradeRepository
    {
        Task<IReadOnlyList<Trade>> GetAsync(DateTime from, DateTime to);

        Task InsertAsync(Trade trade);
    }
}
