using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.EasyBuy.Contract;
using Lykke.Service.EasyBuy.Domain;
using Lykke.Service.EasyBuy.Domain.Services;
using Lykke.Service.EasyBuy.Settings.ServiceSettings;
using Price = Lykke.Service.EasyBuy.Contract.Price;

namespace Lykke.Service.EasyBuy.Rabbit.Publishers
{
    public class PricesPublisher : IPricesPublisher, IDisposable
    {
        private readonly RabbitPublishSettings _publishSettings;
        private readonly ILogFactory _logFactory;
        private readonly ILog _log;
        
        private RabbitMqPublisher<Price> _publisher;

        public PricesPublisher(
            RabbitPublishSettings subscribeSettings,
            ILogFactory logFactory)
        {
            _publishSettings = subscribeSettings;
            _logFactory = logFactory;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_publishSettings.ConnectionString, _publishSettings.ExchangeName);

            _publisher = new RabbitMqPublisher<Price>(_logFactory, settings)
                .SetSerializer(new JsonMessageSerializer<Price>())
                .DisableInMemoryQueuePersistence()
                .Start();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public void Dispose()
        {
            _publisher?.Stop();
        }
        
        public async Task Publish(Domain.Price price)
        {
            try
            {
                await _publisher.ProduceAsync(Mapper.Map<Price>(price));
            }
            catch (Exception e)
            {
                _log.Warning("Could not publish price.", e);
                throw;
            }
        }
    }
}
