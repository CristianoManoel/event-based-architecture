using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ValidationService.Core.Entities;
using ValidationService.Core.Events.Processors;
using ValidationService.Core.Interfaces.Events.Handlers;
using ValidationService.Core.Interfaces.Events.Processors;
using ValidationService.Core.Interfaces.UseCases;
using ValidationService.Core.UseCases;
using ValidationService.Infrastructure.Configurations;
using @Kafka = ValidationService.Infrastructure.Kafka;

namespace ValidationService.Infrastructure.Extensions.DependencyInjection
{
    public static class Extensions
    {

        public static IServiceCollection Register(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICustomerRegistryValidationUseCase, CustomerRegistryValidationUseCase>();
            serviceCollection.AddTransient<INewCustomerRegistryEventProcessor<Customer>, NewCustomerRegistryEventProcessor>();
            serviceCollection.AddTransient<IEventHandler<Customer>, @Kafka.EventHandler<Customer>>();
            
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