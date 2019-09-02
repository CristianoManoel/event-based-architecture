using ServiceBus.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Notification.Core.Entities;
using ClientService.Core.Events.Processors;
using ClientService.Core.UseCases;
using ClientService.Core.Events.Publishers;
using Notification.Infrastructure.ServiceBus.Publishers;

namespace Notification.Infrastructure.Configuration
{
    public static class UseCasesExtensions
    {
        /// <summary>
        /// Register interfaces and implementations on dependency injection framework.
        /// </summary>
        /// <param name="serviceCollection">Instance of ServiceCollection type.</param>
        /// <returns>The given serviceCollection instance with interfaces and implementations registered.</returns>
        public static IServiceCollection AddUseCases(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQuoteRegistrationUseCase, QuoteRegistrationUseCase>();
            serviceCollection.AddTransient<IQuoteReceiveUseCase, QuoteReceiveUseCase>();
            serviceCollection.AddTransient<ICustomerNotificationUseCase, CustomerNotificationUseCase>();
            return serviceCollection;
        }
    }
}