using Notification.Core.Entities;
using System;
using ServiceBus;

namespace Notification.Infrastructure.UseCases.Consumers
{
    public class QuoteOtherConsumer : IEventConsumer<Quote>
    {
        public void Consume(Quote data)
        {
            Console.WriteLine($"[{data.Date}] Quote consumer (2): {data.QuoteName}; {data.Value}");
        }
    }
}
