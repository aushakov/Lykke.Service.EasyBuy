using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Common.Log;
using Lykke.Service.EasyBuy.Client.Api;
using Lykke.Service.EasyBuy.Client.Models.Orders;
using Lykke.Service.EasyBuy.Domain.Exceptions;
using Lykke.Service.EasyBuy.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EasyBuy.Controllers
{
    [Route("/api/[controller]")]
    public class OrdersController : Controller, IOrdersApi
    {
        private readonly IOrdersService _ordersService;
        private readonly ILog _log;

        public OrdersController(
            IOrdersService ordersService,
            ILogFactory logFactory)
        {
            _ordersService = ordersService;
            _log = logFactory.CreateLog(this);
        }

        /// <inheritdoc />
        [HttpPost]
        [ProducesResponseType(typeof(OrderModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<OrderModel> CreateOrderAsync([FromBody] CreateOrderModel model)
        {
            try
            {
                var order = await _ordersService.CreateAsync(model.WalletId, model.PriceId, model.QuotingVolume);

                return Mapper.Map<OrderModel>(order);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Price snapshot does not exist.");
            }
            catch (FailedOperationException exception)
            {
                _log.Error(exception);

                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc />
        [HttpGet("{walletId}/{id}")]
        [ProducesResponseType(typeof(OrderModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<OrderModel> GetOrderAsync(string walletId, string id)
        {
            try
            {
                return Mapper.Map<OrderModel>(await _ordersService.GetAsync(walletId, id));
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Order does not exist.");
            }
        }

        /// <inheritdoc />
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<OrderModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyList<OrderModel>> GetAllOrdersAsync(
            [FromQuery] string walletId,
            [FromQuery] string assetPair,
            [FromQuery] DateTime? timeFrom,
            [FromQuery] DateTime? timeTo,
            [FromQuery] int limit)
        {
            if (limit <= 0)
                throw new ValidationApiException(HttpStatusCode.BadRequest,
                    $"{nameof(limit)} should be positive integer.");

            return Mapper.Map<IReadOnlyList<OrderModel>>(
                await _ordersService.GetAllAsync(walletId, assetPair, timeFrom, timeTo, limit));
        }
    }
}
