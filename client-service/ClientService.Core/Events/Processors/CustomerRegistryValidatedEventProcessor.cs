using System;
using ClientService.Core.Entities;
using ClientService.Core.Interfaces.Events.Processors;

namespace ClientService.Core.Events.Processors
{
    public class CustomerRegistryValidatedEventProcessor : ICustomerRegistryValidatedEventProcessor
    {
        public void Process(Customer data)
        {
            Console.WriteLine($"Costumer validated: [{data.Name}] registry validated with status: [{data.RegisterStatus}]");
        }
    }
}