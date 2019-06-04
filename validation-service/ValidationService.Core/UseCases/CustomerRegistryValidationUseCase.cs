using System;
using ValidationService.Core.Entities;
using ValidationService.Core.Interfaces.UseCases;

namespace ValidationService.Core.UseCases
{
    public class CustomerRegistryValidationUseCase : ICustomerRegistryValidationUseCase
    {
        public void Validate(Customer customer)
        {
            customer.RegisterStatus = Enums.RegisterStatus.Actived;
            Console.Write($"New Customer:[{customer.Name}] registry validated.");
            
        }
    }
}