using System;
using ClientService.Core.Entities;
using ClientService.Core.Enums;
using ClientService.Core.Interfaces;
using ClientService.Core.Interfaces.Events.Handlers;
using ClientService.Core.Interfaces.Events.Processors;
using ClientService.Core.Interfaces.UseCases;
using ClientService.Infrastructure.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientService.ConsoleApp
{
    // Producer
    public class Program
    {

        public static void Main(string[] args)
        {
            var configuration = BuildConfiguration();
            var serviceProvider = BuilderServiceProvider(configuration);

            var registerCustomer = serviceProvider.GetService<IRegisterCustomer>();
            while (true)
            {
                Console.WriteLine("Type the customer name and press enter");
                var customerName = Console.ReadLine();

                Console.WriteLine("Type the customer CPF and press enter");
                var customerCPF = Console.ReadLine();

                var id = Guid.NewGuid();
                var customer = new Customer { Id = id, Cpf = customerCPF, Name = $"{customerName} - {id.ToString()}", RegisterStatus = RegisterStatus.Received };
                                
                registerCustomer.Register(customer);

                Console.Clear();
                Console.WriteLine("Customer receive with sucess. Press enter register another customer:");
                Console.ReadLine();
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
