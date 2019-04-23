using System;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.EasyBuy.Domain.Entities.Orders;
using Lykke.Service.EasyBuy.Domain.Repositories;
using Lykke.Service.EasyBuy.PostgresRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Service.EasyBuy.PostgresRepositories.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private readonly ConnectionFactory _connectionFactory;

        public TransferRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Transfer> GetAsync(string orderId, TransferType type)
        {
            if (!Guid.TryParse(orderId, out Guid id))
                throw new ArgumentException("Invalid identifier", nameof(orderId));

            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                TransferEntity entity = await context.Transfers
                    .SingleOrDefaultAsync(o => o.Id == id && o.Type == type);

                return Mapper.Map<Transfer>(entity);
            }
        }

        public async Task InsertAsync(Transfer transfer)
        {
            using (DataContext context = _connectionFactory.CreateDataContext())
            {
                var entity = Mapper.Map<TransferEntity>(transfer);

                context.Transfers.Add(entity);

                await context.SaveChangesAsync();
            }
        }
    }
}
