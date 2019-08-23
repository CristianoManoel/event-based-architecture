using Notification.Core.Entities;
using System;
using Confluent.Kafka;
using ServiceBus;

namespace Notification.Infrastructure.UseCases.Consumers
{
    public class QuoteConsumer : IEventConsumer<Quote>
    {
        public void Consume(Quote data)
        {
            Console.WriteLine($"[{data.Date}] Quote consumer (1): {data.QuoteName}; {data.Value}");
        }
    }
}
