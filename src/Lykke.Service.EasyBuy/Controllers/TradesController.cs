using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.EasyBuy.Client.Api;
using Lykke.Service.EasyBuy.Client.Models.Trades;
using Lykke.Service.EasyBuy.Domain.Entities.Trades;
using Lykke.Service.EasyBuy.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EasyBuy.Controllers
{
    [Route("/api/[controller]")]
    public class TradesController : Controller, ITradesApi
    {
        private readonly ITradeService _tradeService;

        public TradesController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        /// <inheritdoc />
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<TradeModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<TradeModel>> GetTradesAsync([FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            IReadOnlyList<Trade> trades = await _tradeService.GetAsync(fromDate, toDate);

            return Mapper.Map<IReadOnlyList<TradeModel>>(trades);
        }
    }
}
