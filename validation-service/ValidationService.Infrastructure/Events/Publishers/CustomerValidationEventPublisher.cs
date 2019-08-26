using Microsoft.Extensions.Options;
using ValidationService.Core.Entities;
using ValidationService.Infrastructure.Configuration;
using ValidationService.Infrastructure.Configurations;
using ServiceBus.Kafka;
using ValidationService.Core.Events.Publishers;

namespace ValidationService.Infrastructure.ServiceBus.Publishers
{
    public class CustomerValidationEventPublisher : ICustomerValidationEventPublisher
    {
        private readonly IPublisher _publisher;
        private readonly KafkaOptions.ProducersOptions _publishersOptions;

        public CustomerValidationEventPublisher(
            IOptions<AppConfigurationOptions> configuration,
            IPublisher publisher
        )
        {
            _publisher = publisher;
            _publishersOptions = configuration.Value.Kafka.Producers;
        }

        public void Publish(Customer data)
        {
            _publisher.PublishAsync(_publishersOptions.CustomerValidation, data.Id.ToString(), data);
        }
    }
}
