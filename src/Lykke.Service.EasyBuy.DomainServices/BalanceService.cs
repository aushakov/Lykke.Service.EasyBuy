using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Balances.AutorestClient.Models;
using Lykke.Service.Balances.Client;
using Lykke.Service.EasyBuy.Domain.Entities.Balances;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Services;
using Lykke.Service.EasyBuy.DomainServices.Extensions;

namespace Lykke.Service.EasyBuy.DomainServices
{
    [UsedImplicitly]
    public class BalanceService : IBalancesService
    {
        private readonly IBalancesClient _balancesClient;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public BalanceService(
            IBalancesClient balancesClient,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _balancesClient = balancesClient;
            _settingsService = settingsService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyList<Balance>> GetAsync()
        {
            try
            {
                string walletId = _settingsService.GetWalletId();
                
                IEnumerable<ClientBalanceResponseModel> balance = await _balancesClient.GetClientBalances(walletId);

                return balance.Select(x => new Balance
                {
                    AssetId = x.AssetId,
                    Available = x.Balance,
                    Reserved = x.Reserved
                }).ToList();
            }
            catch (Exception exception)
            {
                _log.WarningWithDetails("An error occurred while retrieving balances", exception);

                throw new FailedOperationException("Failed to retrieve balances.", exception);
            }
        }
    }
}
