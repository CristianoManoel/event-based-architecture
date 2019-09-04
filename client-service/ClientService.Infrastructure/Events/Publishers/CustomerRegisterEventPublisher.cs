using Microsoft.Extensions.Options;
using ClientService.Core.Entities;
using ClientService.Infrastructure.Configuration;
using ClientService.Infrastructure.Configurations;
using ClientService.Core.Events.Publishers;
using ServiceBus;

namespace ClientService.Infrastructure.ServiceBus.Publishers
{
    public class CustomerRegisterEventPublisher : ICustomerRegisterEventPublisher
    {
        private readonly IPublisher _publisher;
        private readonly KafkaOptions.ProducersOptions _publishersOptions;

        public CustomerRegisterEventPublisher(
            IOptions<AppConfigurationOptions> configuration,
            IPublisher publisher
        )
        {
            _publisher = publisher;
            _publishersOptions = configuration.Value.Kafka.Producers;
        }

        public void Publish(Customer data)
        {            
            _publisher.PublishAsync(_publishersOptions.Customer, data.Id.ToString(), data);
        }
    }
}
