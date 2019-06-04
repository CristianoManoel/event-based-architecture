using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValidationService.Core.Entities;
using ValidationService.Core.Interfaces.Events.Handlers;
using ValidationService.Core.Interfaces.Events.Processors;
using ValidationService.Infrastructure.Extensions.DependencyInjection;

namespace ValidationService.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = BuildConfiguration();
            var serviceProvider = BuilderServiceProvider(configuration);
            var newCustomerRegistryEvenHandler = serviceProvider.GetService<IEventHandler<Customer>>();
            var newCustomerRegistryEvenProcessor = serviceProvider.GetService<INewCustomerRegistryEventProcessor<Customer>>();
            newCustomerRegistryEvenHandler.ConsumeEvents(nameof(Customer), newCustomerRegistryEvenProcessor);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, true)
                .Build();
        }

        private static ServiceProvider BuilderServiceProvider(IConfigurationRoot configuration)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.Register();
            serviceCollection.RegisterConfigurationOptions(configuration);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
