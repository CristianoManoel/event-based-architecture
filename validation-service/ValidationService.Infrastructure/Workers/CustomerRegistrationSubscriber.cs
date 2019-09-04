using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ValidationService.Infrastructure.Configuration;
using ValidationService.Infrastructure.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ValidationService.Core.Events.Processors;
using ServiceBus;

namespace ValidationService.Infrastructure.Workers
{
    public class CustomerRegistrationSubscriber : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ISubscriber _subscriber;
        private readonly ICustomerRegistrationEventProcessor _event;
        private KafkaOptions.ConsumersOptions _subscribersOptions;

        public CustomerRegistrationSubscriber(
            IConfiguration configuration,
            ILogger<CustomerRegistrationSubscriber> logger,
            IOptions<AppConfigurationOptions> appConfiguration,
            ISubscriber subscriberCustomer,
            ICustomerRegistrationEventProcessor eventConsumer
        )
        {
            _logger = logger;
            _configuration = configuration;
            _subscriber = subscriberCustomer;
            _subscribersOptions = appConfiguration.Value.Kafka.Consumers;
            _event = eventConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (CanSubscribe())
            {
                _logger.LogInformation("Start customer consumer");

                Action<Exception> errorHandler = e =>
                {
                    if (e is ConsumeException ce)
                        Console.WriteLine($"Error occured: {ce.Error.Reason}");
                    else
                        Console.WriteLine($"Error occured: {e.ToString()}");
                };

                _subscriber.SubscribeAsync(_subscribersOptions.Customer, _event, errorHandler);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop customer consumer");
            return Task.CompletedTask;
        }

        private bool CanSubscribe()
        {
            return _configuration["disable-consumers"] == null;
        }

        public void Dispose()
        {

        }
    }
}
