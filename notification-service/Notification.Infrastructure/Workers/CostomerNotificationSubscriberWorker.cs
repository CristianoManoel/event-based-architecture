using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Configuration;
using Notification.Infrastructure.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;
using ServiceBus.Kafka;
using ClientService.Core.Events.Processors;
using ServiceBus;

namespace Notification.Infrastructure.Workers
{
    public class CostomerNotificationSubscriberWorker : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISubscriber _subscriber;
        private readonly AppConfigurationOptions _configuration;
        private KafkaOptions.ConsumersOptions _subscribersOptions;
        private readonly ICustomerNotificationEventProcessor _event;

        public CostomerNotificationSubscriberWorker(
            ILogger<CostomerNotificationSubscriberWorker> logger,
            IOptions<AppConfigurationOptions> configuration,
            ISubscriber subscriber,
            ICustomerNotificationEventProcessor eventConsumer
        )
        {
            _logger = logger;
            _subscriber = subscriber;
            _configuration = configuration.Value;
            _subscribersOptions = _configuration.Kafka.Consumers;
            _event = eventConsumer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start customer consumer");

            Action<Exception> errorHandler = e =>
            {
                if (e is ConsumeException ce)
                    Console.WriteLine($"Error occured: {ce.Error.Reason}");
                else
                    Console.WriteLine($"Error occured: {e.ToString()}");
            };

            _subscriber.SubscribeAsync(_subscribersOptions.CustomerNotification, _event, errorHandler);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop Kafka auto consumer");
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
