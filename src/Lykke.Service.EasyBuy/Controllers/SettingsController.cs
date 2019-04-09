using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.EasyBuy.Client.Api;
using Lykke.Service.EasyBuy.Client.Models.Settings;
using Lykke.Service.EasyBuy.Domain.Entities.Settings;
using Lykke.Service.EasyBuy.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EasyBuy.Controllers
{
    [Route("/api/[controller]")]
    public class SettingsController : Controller, ISettingsApi
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        /// <inheritdoc/>
        [HttpGet("account")]
        [ProducesResponseType(typeof(AccountSettingsModel), (int) HttpStatusCode.OK)]
        public Task<AccountSettingsModel> GetAccountSettingsAsync()
        {
            return Task.FromResult(new AccountSettingsModel
            {
                WalletId = _settingsService.GetWalletId()
            });
        }

        /// <inheritdoc/>
        /// <response code="200">The model that represents timers settings.</response>
        [HttpGet("timers")]
        [ProducesResponseType(typeof(TimersSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<TimersSettingsModel> GetTimersSettingsAsync()
        {
            TimersSettings timersSettings = await _settingsService.GetTimersSettingsAsync();

            return Mapper.Map<TimersSettingsModel>(timersSettings);
        }

        /// <inheritdoc/>
        /// <response code="204">The timers settings successfully updated.</response>
        [HttpPut("timers")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task UpdateTimersSettingsAsync([FromBody] TimersSettingsModel model)
        {
            var timersSettings = Mapper.Map<TimersSettings>(model);

            await _settingsService.UpdateTimersSettingsAsync(timersSettings);
        }
    }
}
