using Microsoft.Extensions.DependencyInjection;
using ValidationService.Core.Entities;
using ServiceBus.Kafka;
using ValidationService.Core.UseCases;
using ValidationService.Core.Events.Publishers;
using ValidationService.Infrastructure.ServiceBus.Publishers;
using ValidationService.Core.Events.Processors;

namespace ValidationService.Infrastructure.Configuration
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
            serviceCollection.AddTransient<ICustomerRegistrationEventProcessor, CustomerRegistrationEventProcessor>();

            // Publishers
            serviceCollection.AddTransient<IPublisher, KafkaServiceBus>();
            serviceCollection.AddTransient<ICustomerValidationEventPublisher, CustomerValidationEventPublisher>();

            // Use cases (core services)
            serviceCollection.AddTransient<ICustomerValidationUseCase, CustomerValidationUseCase>();

            return serviceCollection;
        }
    }
}