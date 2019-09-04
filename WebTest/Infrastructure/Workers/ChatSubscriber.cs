using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ServiceBus;
using WebTest.Infrastructure.Configuration;
using WebTest.Infrastructure.Configurations;
using ServiceBus.Events;
using WebTest.Core.Entities;
using Microsoft.AspNetCore.SignalR;
using WebTest.Hubs;
using Newtonsoft.Json;

namespace WebTest.Infrastructure.Workers
{
    public class ChatSubscriber : IHostedService, IDisposable, IEventProcessor<ChatMessage>
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ISubscriber _subscriber;
        private KafkaOptions.ConsumersOptions _subscribersOptions;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatSubscriber(
            IConfiguration configuration,
            ILogger<ChatSubscriber> logger,
            IOptions<AppConfigurationOptions> appConfiguration,
            ISubscriber subscriber,
            IHubContext<ChatHub> hubContext
        )
        {
            _logger = logger;
            _configuration = configuration;
            _subscriber = subscriber;
            _subscribersOptions = appConfiguration.Value.Kafka.Consumers;
            _hubContext = hubContext;
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

            _subscriber.SubscribeAsync(_subscribersOptions.Chat, this, errorHandler);

            return Task.CompletedTask;
        }

        public void Subscribe(ChatMessage data)
        {
            _hubContext.Clients.All.SendAsync("chat", data);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stop customer consumer");
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
