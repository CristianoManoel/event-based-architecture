﻿using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Core.Entities;
using Notification.Infrastructure.Configuration;
using Notification.Infrastructure.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ServiceBus.Kafka;
using ClientService.Core.Events.Subscribers;

namespace Notification.Infrastructure.Workers
{
    public class QuotesSubscriberWorker : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISubscriber _subscriber;
        private readonly AppConfigurationOptions _configuration;
        private KafkaOptions.ConsumersOptions _subscribersOptions;
        private readonly IEnumerable<IQuoteEventSubscriber> _events;

        public QuotesSubscriberWorker(
            ILogger<CostomerNotificationSubscriberWorker> logger,
            IOptions<AppConfigurationOptions> configuration,
            ISubscriber subscriber,
            IEnumerable<IQuoteEventSubscriber> events
        )
        {
            _logger = logger;
            _subscriber = subscriber;
            _configuration = configuration.Value;
            _subscribersOptions = _configuration.Kafka.Consumers;
            _events = events;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start quotes consumer");

            Action<Exception> errorHandler = e =>
            {
                if (e is ConsumeException ce)
                    Console.WriteLine($"Error occured: {ce.Error.Reason}");
                else
                    Console.WriteLine($"Error occured: {e.ToString()}");
            };

            _subscriber.SubscribeAsync(_subscribersOptions.Quotes, _events, errorHandler);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop quotes consumer");
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
