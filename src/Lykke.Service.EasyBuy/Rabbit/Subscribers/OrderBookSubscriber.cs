using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.EasyBuy.Domain.Services;
using OrderBook = Lykke.Common.ExchangeAdapter.Contracts.OrderBook;

namespace Lykke.Service.EasyBuy.Rabbit.Subscribers
{
    [UsedImplicitly]
    public class OrderBookSubscriber : IDisposable
    {
        private readonly string _exchangeName;
        private readonly string _connectionString;
        private readonly string _queueSuffix;
        private readonly IOrderBookService _orderBookService;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;

        private RabbitMqSubscriber<OrderBook> _subscriber;
        
        public OrderBookSubscriber(
            string exchangeName,
            string connectionString,
            string queueSuffix,
            IOrderBookService orderBookService,
            ILogFactory logFactory)
        {
            _exchangeName = exchangeName;
            _orderBookService = orderBookService;
            _logFactory = logFactory;
            _connectionString = connectionString;
            _queueSuffix = queueSuffix;

            _log = logFactory.CreateLog(this);
        }
        
        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForSubscriber(_connectionString, _exchangeName, _queueSuffix);

            settings.DeadLetterExchangeName = null;
            settings.IsDurable = false;

            _subscriber = new RabbitMqSubscriber<OrderBook>(_logFactory, settings,
                    new ResilientErrorHandlingStrategy(_logFactory, settings, TimeSpan.FromSeconds(10)))
                .SetMessageDeserializer(new JsonMessageDeserializer<OrderBook>())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .Subscribe(ProcessMessageAsync)
                .CreateDefaultBinding()
                .Start();
        }

        public void Stop()
        {
            _subscriber?.Stop();
        }

        public void Dispose()
        {
            _subscriber?.Dispose();
        }

        private async Task ProcessMessageAsync(OrderBook orderBook)
        {
            try
            {
                await _orderBookService.HandleAsync(orderBook.Source, Mapper.Map<Domain.OrderBook>(orderBook));
            }
            catch (Exception exception)
            {
                _log.Error(exception, "An error occurred during processing lykke order book", orderBook);
            }
        }
    }
}
