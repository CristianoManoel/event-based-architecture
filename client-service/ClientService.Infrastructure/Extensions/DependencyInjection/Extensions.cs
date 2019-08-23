using ClientService.Core.Entities;
using ClientService.Core.Events.Processors;
using ClientService.Core.Interfaces.Events.Handlers;
using ClientService.Core.Interfaces.Events.Processors;
using ClientService.Core.Interfaces.Events.Publishers;
using ClientService.Core.Interfaces.UseCases;
using ClientService.Core.UseCases;
using ClientService.Infrastructure.Configurations;
using @Kafka = ClientService.Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ServiceBus.Kafka;

namespace ClientService.Infrastructure.Extensions.DependencyInjection
{
    public static class Extensions
    {
        /// <summary>
        /// Register interfaces and implementations on dependency injection framework.
        /// </summary>
        /// <param name="serviceCollection">Instance of ServiceCollection type.</param>
        /// <returns>The given serviceCollection instance with interfaces and implementations registered.</returns>
        public static IServiceCollection Register(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IRegisterCustomer, RegisterCustomer>();
            serviceCollection.AddTransient<IEventPublisher<Customer>, @Kafka.EventPublisher<Customer>>();

            // Test
            serviceCollection.AddTransient<KafkaProducer>();

            serviceCollection.AddTransient<IEventHandler<Customer>, @Kafka.EventHandler<Customer>>();
            serviceCollection.AddTransient<ICustomerRegistryValidatedEventProcessor, CustomerRegistryValidatedEventProcessor>();

            return serviceCollection;
        }
        public static IServiceCollection RegisterConfigurationOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<ConfigurationOptions>(configuration);
            serviceCollection.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<ConfigurationOptions>>().Value);

            serviceCollection.Configure<KafkaOptions>(configuration.GetSection("Kafka"));
            serviceCollection.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<KafkaOptions>>().Value);

            return serviceCollection;
        }

    }
}