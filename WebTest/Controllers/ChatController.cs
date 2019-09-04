using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceBus;
using ServiceBus.Configurations;
using ServiceBus.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebTest.Core.Entities;
using WebTest.Hubs;
using WebTest.Infrastructure.Configurations;

namespace WebTest.Controllers
{
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private static Dictionary<string, CancellationTokenSource> subscribersCancels = new Dictionary<string, CancellationTokenSource>();
        public static bool EnableTestError;

        private readonly IPublisher _publisher;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly AppConfigurationOptions _appConfiguration;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            ILogger<ChatController> logger,
            IPublisher publisher,
            IOptions<AppConfigurationOptions> appConfiguration,
            IHubContext<ChatHub> hubContext
        )
        {
            this._publisher = publisher;
            this._hubContext = hubContext;
            this._appConfiguration = appConfiguration.Value;
            this._logger = logger;
        }


        [HttpGet("[action]")]
        public async Task<bool> EnableError(bool enable)
        {
            EnableTestError = enable;
            return EnableTestError;
        }

        [HttpPost("[action]")]
        public async Task<bool> Send([FromBody]PublisherRequest request)
        {
            request.Message.Id = Guid.NewGuid();
            request.Message.SendDate = DateTime.Now;

            await this._publisher.PublishAsync(request.Settings, request.Message.Id.ToString(), request.Message);
            return true;
        }

        [HttpPost("[action]")]
        public IActionResult Subscribe(string roomId, [FromBody]ConsumerSettings settings, [FromServices] ISubscriber subscriber)
        {
            if (!subscribersCancels.ContainsKey(roomId))
            {
                Action<Exception> errorHandler = e =>
                {
                    if (e is ConsumeException ce)
                        Console.WriteLine($"Error occured: {ce.Error.Reason}");
                    else
                        Console.WriteLine($"Error occured: {e.ToString()}");
                };

                var cancelSource = new CancellationTokenSource();
                subscriber.SubscribeAsync(settings, new ChatEventProcessor(roomId, _hubContext, this._logger), errorHandler, cancelSource.Token);
                subscribersCancels.Add(roomId, cancelSource);
            }

            return Ok(new { Message = "Request Completed" });
        }

        [HttpGet("[action]")]
        public IActionResult UnSubscribe(string roomId)
        {
            subscribersCancels[roomId].Cancel();
            subscribersCancels.Remove(roomId);
            return Ok(new { Message = "Request Completed" });
        }

        public class PublisherRequest
        {
            public ProducerSettings Settings { get; set; }
            public ChatMessage Message { get; set; }
        }

        private class ChatEventProcessor : IEventProcessor<ChatMessage>
        {
            private readonly string _roomId;
            private readonly IHubContext<ChatHub> _hubContext;
            private readonly ILogger<ChatController> _logger;

            public ChatEventProcessor(string roomId, IHubContext<ChatHub> _hubContext, ILogger<ChatController> logger)
            {
                this._roomId = roomId;
                this._hubContext = _hubContext;
                this._logger = logger;
            }

            public void Subscribe(ChatMessage data)
            {
                try
                {
                    if (EnableTestError && data.Message.ToLower() == "error")
                        throw new Exception("Ocorreu um erro");

                    this._logger.LogInformation($"Consumer: {this._roomId}; Message: {data.Message}");

                    _hubContext.Clients.Group(this._roomId).SendAsync("chat", data);
                }
                catch
                {
                    _hubContext.Clients.Group(this._roomId).SendAsync("chatError", data);
                    throw;
                }
            }
        }
    }
}
