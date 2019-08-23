using Notification.Core.Entities;
using System;
using Newtonsoft.Json;
using ServiceBus;

namespace Notification.Infrastructure.UseCases.Consumers
{
    public class NotificationConsumer : IEventConsumer<NotificationInfo>
    {
        public void Consume(NotificationInfo data)
        {
            Console.WriteLine(JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
