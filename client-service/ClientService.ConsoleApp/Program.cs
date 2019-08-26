using ClientService.Core.Entities;
using ClientService.Core.Enums;
using ClientService.Core.UseCases;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ClientService.ConsoleApp
{
    public class Program
    {
        private const string _prefix = "CLIENT_SERVICE_";
        private const string _appsettings = "appsettings.json";
        private const string _hostsettings = "hostsettings.json";

        #region Start application without hosted services

        //public static void Main(string[] args)
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .AddJsonFile($"appsettings.json", false, true)
        //        .Build();

        //    var serviceCollection = new ServiceCollection();

        //    serviceCollection.AddTransient<Program>();
        //    serviceCollection.AddTransient<IConfiguration>(c => configuration);

        //    new Startup(configuration).ConfigureServices(serviceCollection);

        //    serviceCollection.BuildServiceProvider().GetService<Program>().Run(args);
        //}

        #endregion

        #region Start application with hosted services

        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile(_hostsettings, optional: true);
                    configHost.AddEnvironmentVariables(prefix: _prefix);
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile(_appsettings, optional: true);
                    configApp.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                    configApp.AddEnvironmentVariables(prefix: _prefix);
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    services.AddTransient<Program>();
                    new Startup(hostContext.Configuration).ConfigureServices(services);
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                })
                .UseConsoleLifetime()
                .Build();

            host.RunAsync();
            host.Services.GetService<Program>().Run();
        }

        #endregion

        private readonly ICustomerRegistrationUseCase _registerCustomer;

        public Program(ICustomerRegistrationUseCase registerCustomer)
        {
            _registerCustomer = registerCustomer;
        }

        public void Run(params string[] args)
        {
            while (true)
            {
                Console.WriteLine("Type the customer name and press enter");
                var customerName = Console.ReadLine();

                Console.WriteLine("Type the customer CPF and press enter");
                var customerCPF = Console.ReadLine();

                var id = Guid.NewGuid();
                var customer = new Customer { Id = id, Cpf = customerCPF, Name = $"{customerName} - {id.ToString()}", RegisterStatus = RegisterStatus.Received };

                _registerCustomer.Register(customer);

                Console.WriteLine("--------------------------------------------");
            }
        }
    }
}