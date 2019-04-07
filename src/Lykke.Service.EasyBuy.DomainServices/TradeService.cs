using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EasyBuy.Domain.Entities.Trades;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.DomainServices
{
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _tradeRepository;

        public TradeService(ITradeRepository tradeRepository)
        {
            _tradeRepository = tradeRepository;
        }

        public Task<IReadOnlyList<Trade>> GetAsync(DateTime from, DateTime to)
        {
            return _tradeRepository.GetAsync(from, to);
        }

        public Task AddAsync(Trade trade)
        {
            return _tradeRepository.InsertAsync(trade);
        }
    }
}
