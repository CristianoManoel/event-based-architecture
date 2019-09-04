using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ClientService.Core.Entities;
using ClientService.Infrastructure.Configuration;
using ClientService.Infrastructure.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ClientService.Core.Events.Processors;
using ServiceBus;

namespace ClientService.Infrastructure.Workers
{
    public class CustomerValidationSubscriber : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ISubscriber _subscriber;
        private readonly ICustomerValidationEventProcessor _event;
        private KafkaOptions.ConsumersOptions _subscribersOptions;

        public CustomerValidationSubscriber(
            IConfiguration configuration,
            ILogger<CustomerValidationSubscriber> logger,
            IOptions<AppConfigurationOptions> appConfiguration,
            ISubscriber subscriber,
            ICustomerValidationEventProcessor eventConsumer
        )
        {
            _logger = logger;
            _configuration = configuration;
            _subscriber = subscriber;
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

                _subscriber.SubscribeAsync(_subscribersOptions.CustomerValidation, _event, errorHandler);
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
