using Microsoft.Extensions.DependencyInjection;
using ServiceBus;
using ServiceBus.Kafka;

namespace WebTest.Infrastructure.Configuration
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
            //serviceCollection.AddTransient<ICustomerValidationEventProcessor, CustomerValidationEventProcessor>();

            // Publishers
            serviceCollection.AddTransient<IPublisher, KafkaServiceBus>();
            //serviceCollection.AddTransient<ICustomerRegisterEventPublisher, CustomerRegisterEventPublisher>();
            //serviceCollection.AddTransient<ICustomerNotificationEventPublisher, CustomerNotificationEventPublisher>();

            // Use cases (core services)
            //serviceCollection.AddTransient<ICustomerRegistrationUseCase, CustomerRegistrationUseCase>();
            //serviceCollection.AddTransient<ICustomerSendNotificationUseCase, CustomerSendNotificationUseCase>();
            
            return serviceCollection;
        }
    }
}