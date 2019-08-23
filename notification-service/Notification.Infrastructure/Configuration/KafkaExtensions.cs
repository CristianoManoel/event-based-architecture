using ServiceBus;
using ServiceBus.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Notification.Core.Entities;
using Notification.Infrastructure.UseCases.Consumers;
using Notification.Infrastructure.UseCases.Producers;
using Notification.Infrastructure.Workers;

namespace Notification.Infrastructure.Configuration
{
    public static class KafkaExtensions
    {
        /// <summary>
        /// Register interfaces and implementations on dependency injection framework.
        /// </summary>
        /// <param name="serviceCollection">Instance of ServiceCollection type.</param>
        /// <returns>The given serviceCollection instance with interfaces and implementations registered.</returns>
        public static IServiceCollection AddKafka(this IServiceCollection serviceCollection)
        {
            // Consumers
            serviceCollection.AddTransient<IProducer, KafkaProducer>();
            serviceCollection.AddTransient<IEventConsumer<Quote>, QuoteConsumer>();
            serviceCollection.AddTransient<IEventConsumer<Quote>, QuoteOtherConsumer>();
            serviceCollection.AddTransient<IEventConsumer<NotificationInfo>, NotificationConsumer>();

            // Producers
            serviceCollection.AddTransient<IConsumer, KafkaConsumer>();
            serviceCollection.AddTransient<IEventProducer<Quote>, QuoteProducer>();

            return serviceCollection;
        }
    }
}