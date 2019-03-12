using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Services;
using Lykke.Service.ExchangeOperations.Client;
using Lykke.Service.ExchangeOperations.Client.Models;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class InternalTransfersService : IInternalTransfersService
    {
        private readonly IExchangeOperationsServiceClient _exchangeOperationsServiceClient;

        public InternalTransfersService(
            IExchangeOperationsServiceClient exchangeOperationsServiceClient)
        {
            _exchangeOperationsServiceClient = exchangeOperationsServiceClient;
        }
        
        public async Task TransferAsync(
            string transferId,
            string sourceWalletId,
            string destinationWalletId,
            string assetId,
            decimal amount)
        {
            ExchangeOperationResult result;
            
            try
            {
                result = await _exchangeOperationsServiceClient.ExchangeOperations.TransferAsync(
                    new TransferRequestModel
                    {
                        SourceClientId = sourceWalletId,
                        DestClientId = destinationWalletId,
                        Amount = (double) amount,
                        AssetId = assetId,
                        OperationId = transferId
                    });
            }
            catch (Exception e)
            {
                throw new FailedOperationException("ME call failed.", e);
            }
            

            if (!result.IsOk())
            {
                if (result.Code == 401)
                {
                    throw new MeNotEnoughFundsException(transferId);
                }
                else
                {
                    throw new MeOperationException(transferId, result.Code);
                }
            }
        }
    }
}
