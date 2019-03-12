using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Balances.Client;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Services;

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
                var balance = await _balancesClient.GetClientBalances(await _settingsService.GetWalletIdAsync());

                return balance.Select(x => new Balance
                {
                    AssetId = x.AssetId,
                    Available = x.Balance,
                    Reserved = x.Reserved
                }).ToArray();
            }
            catch (Exception e)
            {
                throw new FailedOperationException("Failed to retrieve balances.", e);
            }
        }
    }
}
