using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.EasyBuy.Client.Api;
using Lykke.Service.EasyBuy.Client.Models;
using Lykke.Service.EasyBuy.Domain;
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
        public async Task<AccountSettingsModel> GetAccountSettingsAsync()
        {
            return new AccountSettingsModel
            {
                WalletId = await _settingsService.GetWalletIdAsync()
            };
        }

        /// <inheritdoc/>
        [HttpGet("default")]
        [ProducesResponseType(typeof(DefaultSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<DefaultSettingsModel> GetDefaultSettingsAsync()
        {
            return Mapper.Map<DefaultSettingsModel>(await _settingsService.GetDefaultSettingsAsync());
        }

        /// <inheritdoc/>
        [HttpPut("default")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task UpdateDefaultPriceSettingsAsync([FromBody] DefaultSettingsModel defaultSettings)
        {
            await _settingsService.UpdateDefaultSettingsAsync(Mapper.Map<DefaultSetting>(defaultSettings));
        }
    }
}
