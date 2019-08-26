using Microsoft.Extensions.Options;
using ServiceBus.Kafka;
using ClientService.Core.Events.Publishers;
using Notification.Infrastructure.Configurations;
using Notification.Core.Entities;
using Notification.Infrastructure.Configuration;

namespace Notification.Infrastructure.ServiceBus.Publishers
{
    public class QuoteEventPublisher : IQuoteEventPublisher
    {
        private readonly IPublisher _publisher;
        private readonly KafkaOptions.ProducersOptions _publishersOptions;

        public QuoteEventPublisher(
            IOptions<AppConfigurationOptions> configuration,
            IPublisher publisher
        )
        {
            _publisher = publisher;
            _publishersOptions = configuration.Value.Kafka.Producers;
        }

        public void Publish(Quote data)
        {
            _publisher.PublishAsync(_publishersOptions.Quotes, data.QuoteName, data);
        }
    }
}
