using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientService.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, true)
                .Build();

            var serviceCollection = new ServiceCollection();
        }
    }
}
