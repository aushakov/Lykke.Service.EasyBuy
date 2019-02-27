using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using Lykke.Service.EasyBuy.Client.Api;
using Lykke.Service.EasyBuy.Client.Models;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EasyBuy.Controllers
{
    [Route("/api/[controller]")]
    public class BalancesController : Controller, IBalancesApi
    {
        private readonly IBalancesService _balancesService;
        private readonly ILog _log;
        
        public BalancesController(
            IBalancesService balancesService,
            ILogFactory logFactory)
        {
            _balancesService = balancesService;
            _log = logFactory.CreateLog(this);
        }
        
        /// <inheritdoc/>
        /// <response code="200">A collection of instruments.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<BalanceModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<BalanceModel>> GetAsync()
        {
            try
            {
                return Mapper.Map<IReadOnlyList<BalanceModel>>(await _balancesService.GetAsync());
            }
            catch (FailedOperationException exception)
            {
                _log.Error(exception);
                
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }
    }
}
