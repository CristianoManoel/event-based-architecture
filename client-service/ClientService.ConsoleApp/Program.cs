using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientService.Core.Entities;
using ClientService.Core.Enums;
using ClientService.Core.Interfaces;
using ClientService.Core.Interfaces.Events.Handlers;
using ClientService.Core.Interfaces.Events.Processors;
using ClientService.Core.Interfaces.UseCases;
using ClientService.Infrastructure.Extensions.DependencyInjection;
using ClientService.Infrastructure.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientService.ConsoleApp
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var configuration = BuildConfiguration();
            var serviceProvider = BuilderServiceProvider(configuration);
            CustomerConsumer(serviceProvider);
            CustomerProducer(serviceProvider);
            Console.Read();
        }
        private static void CustomerConsumer(ServiceProvider serviceProvider)
        {
            var customerRegistryValidationEventHandler = serviceProvider.GetService<IEventHandler<Customer>>();
            var customerRegistryValidatedEventProcessor = serviceProvider.GetService<ICustomerRegistryValidatedEventProcessor>();
            customerRegistryValidationEventHandler.ConsumeEvents("Customer-Validation", customerRegistryValidatedEventProcessor);
        }

        private static void CustomerProducer(ServiceProvider serviceProvider)
        {
            IRegisterCustomer registerCustomer;
            List<Customer> list;

            registerCustomer = serviceProvider.GetService<IRegisterCustomer>();

            while (true)
            {
                Console.WriteLine("Entre com a quantidade de mensagens:");
                var repeat = Convert.ToInt32(Console.ReadLine());
                list = new List<Customer>();

                for (var i = 1; i <= repeat; i++)
                {
                    var customerName = "customerName-" + i;
                    var customerCPF = "customerCPF-" + i;
                    var id = Guid.NewGuid();
                    list.Add(new Customer { Id = i, Cpf = customerCPF, Name = $"{customerName} - {id.ToString()}", RegisterStatus = RegisterStatus.Received });
                }

                registerCustomer.Register(list).Wait();
            }
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
