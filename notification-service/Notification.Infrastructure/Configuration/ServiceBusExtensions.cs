using ServiceBus.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Notification.Core.Entities;
using ClientService.Core.Events.Subscribers;
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
            serviceCollection.AddTransient<ICustomerNotificationEventSubscriber, CustomerNotificationEventSubscriber>();
            serviceCollection.AddTransient<IQuoteEventSubscriber, QuoteEventSubscriber>();

            // Publishers
            serviceCollection.AddTransient<IPublisher, KafkaServiceBus>();
            serviceCollection.AddTransient<IQuoteEventPublisher, QuoteEventPublisher>();            

            return serviceCollection;
        }
    }
}