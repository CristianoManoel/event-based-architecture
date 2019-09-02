using ServiceBus.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Notification.Core.Entities;
using ClientService.Core.Events.Processors;
using ClientService.Core.UseCases;
using ClientService.Core.Events.Publishers;
using Notification.Infrastructure.ServiceBus.Publishers;

namespace Notification.Infrastructure.Configuration
{
    public static class ServiceBusExtensions
    {
        /// <summary>
        /// Register interfaces and implementations on dependency injection framework.
        /// </summary>
        /// <param name="serviceCollection">Instance of ServiceCollection type.</param>
        /// <returns>The given serviceCollection instance with interfaces and implementations registered.</returns>
        public static IServiceCollection AddServiceBus(this IServiceCollection serviceCollection)
        {
            // Subscribers
            serviceCollection.AddTransient<ISubscriber, KafkaServiceBus>();
            serviceCollection.AddTransient<ICustomerNotificationEventProcessor, CustomerNotificationEventProcessor>();
            serviceCollection.AddTransient<IQuoteEventProcessor, QuoteEventProcessor>();

            // Publishers
            serviceCollection.AddTransient<IPublisher, KafkaServiceBus>();
            serviceCollection.AddTransient<IQuoteEventPublisher, QuoteEventPublisher>();            

            return serviceCollection;
        }
    }
}