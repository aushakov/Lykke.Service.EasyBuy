using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.EasyBuy.Domain.Entities.Settings;
using Lykke.Service.EasyBuy.Domain.Services;

namespace Lykke.Service.EasyBuy.Timers
{
    [UsedImplicitly]
    public class OrdersTimer : Timer
    {
        private readonly ISettingsService _settingsService;
        private readonly IOrdersService _ordersService;

        public OrdersTimer(
            ISettingsService settingsService,
            IOrdersService ordersService,
            ILogFactory logFactory)
        {
            _settingsService = settingsService;
            _ordersService = ordersService;

            Log = logFactory.CreateLog(this);
        }

        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _ordersService.ProcessPendingAsync();
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimersSettings timersSettings = await _settingsService.GetTimersSettingsAsync();

            return timersSettings.Orders;
        }
    }
}
