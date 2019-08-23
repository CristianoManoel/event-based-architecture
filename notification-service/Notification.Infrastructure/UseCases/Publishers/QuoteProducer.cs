using ServiceBus;
using Microsoft.Extensions.Options;
using Notification.Core.Entities;
using Notification.Infrastructure.Configuration;
using Notification.Infrastructure.Configurations;
using System;

namespace Notification.Infrastructure.UseCases.Producers
{
    public class QuoteProducer : IEventProducer<Quote>
    {
        private readonly IProducer _producer;
        private readonly KafkaOptions.ProducersOptions _producersOptions;

        public QuoteProducer(
            IOptions<ConfigurationOptions> configuration,
            IProducer producer
        )
        {
            _producer = producer;
            _producersOptions = configuration.Value.Kafka.Producers;
        }

        public void Produce(Quote data)
        {
            Console.WriteLine($"Produced Quote: {data.QuoteName}; {data.Value}");

            var key = DateTime.Now.ToString();
            _producer
                    .WithConfig(_producersOptions.Quotes)
                    .Produce(key, data);
        }
    }
}
