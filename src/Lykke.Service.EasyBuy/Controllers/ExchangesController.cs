using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Common.PasswordTools;
using Lykke.Service.EasyBuy.Client.Api;
using Lykke.Service.EasyBuy.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EasyBuy.Controllers
{
    [Route("/api/[controller]")]
    public class ExchangesController : Controller, IExchangesApi
    {
        private readonly IOrderBookService _orderBookService;
        
        public ExchangesController(IOrderBookService orderBookService)
        {
            _orderBookService = orderBookService;
        }

        /// <inheritdoc/>
        /// <response code="200">A list of available exchanges.</response>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IReadOnlyList<string>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyList<string>> GetAvailableAsync()
        {
            return Task.FromResult(_orderBookService.GetExistingExchanges());
        }
    }
}
