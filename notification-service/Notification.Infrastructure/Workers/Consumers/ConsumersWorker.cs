using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Core.Entities;
using Notification.Infrastructure.Configuration;
using Notification.Infrastructure.Configurations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ServiceBus;
using System.Collections.Generic;

namespace Notification.Infrastructure.Workers
{
    public class ConsumersWorker : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IConsumer _consumer;
        private readonly ConfigurationOptions _configuration;
        private KafkaOptions.ConsumersOptions _consumersOptions;
        private readonly ConsumerMap _consumerMap;

        public ConsumersWorker(
            ILogger<ConsumersWorker> logger,
            IOptions<ConfigurationOptions> configuration,
            IConsumer consumer,
            IEventConsumer<NotificationInfo> notificationConsumer,
            IEnumerable<IEventConsumer<Quote>> quotesConsumer
        )
        {
            _logger = logger;
            _consumer = consumer;
            _configuration = configuration.Value;
            _consumersOptions = _configuration.Kafka.Consumers;

            _consumerMap = new ConsumerMap();
            _consumerMap.Add(_consumersOptions.Quotes, quotesConsumer);
            _consumerMap.Add(_consumersOptions.Notification, notificationConsumer);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartConsumers();
            return Task.CompletedTask;
        }

        private void StartConsumers()
        {
            _logger.LogInformation("Start kafka auto consumer");

            foreach (var config in _consumersOptions)
            {
                // Adiciona uma Task de consumir caso exista "ouvintes" para 
                // essa configuração e também se a flag "AutoConsume" ela está ligada
                var consumers = _consumerMap.GetConsumers(config);
                if (consumers.Any() && config.AutoConsume)
                {
                    _consumer
                        .WithConfig(config)
                        .Success(consumers)
                        .Error(e =>
                        {
                            if (e is ConsumeException ce)
                                Console.WriteLine($"Error occured: {ce.Error.Reason}");
                            else
                                Console.WriteLine($"Error occured: {e.ToString()}");
                        })
                        .Subscribe();
                }
            }
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
