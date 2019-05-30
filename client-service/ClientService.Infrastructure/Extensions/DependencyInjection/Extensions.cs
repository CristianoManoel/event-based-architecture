using ClientService.Core.Entities;
using ClientService.Core.Interfaces;
using ClientService.Core.Interfaces.UseCases;
using ClientService.Core.UseCases;
using ClientService.Infrastructure.Configurations;
using ClientService.Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            serviceCollection.AddTransient<IEventPublisher<Customer>, EventPublisher<Customer>>();

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