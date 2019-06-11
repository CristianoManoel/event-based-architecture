using System;
using ValidationService.Core.Entities;
using ValidationService.Core.Interfaces.Events.Publishers;
using ValidationService.Core.Interfaces.UseCases;

namespace ValidationService.Core.UseCases
{
    public class CustomerRegistryValidationUseCase : ICustomerRegistryValidationUseCase
    {
        private readonly IEventPublisher<Customer> eventPublisher;
        const string Topic = "Customer-Validation" ;
        public CustomerRegistryValidationUseCase(IEventPublisher<Customer> eventPublisher)
        {
            this.eventPublisher = eventPublisher;
        }
        public void Validate(Customer customer)
        {
            customer.RegisterStatus = Enums.RegisterStatus.Actived;
            Console.Write($"New Customer:[{customer.Name}] registry validated.");
            eventPublisher.PublishEvent(Topic, customer.Id.ToString(), customer);
        }
    }
}