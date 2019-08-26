using ClientService.Core.Entities;
using ClientService.Core.Events.Publishers;
using System;

namespace ClientService.Core.UseCases
{
    public class CustomerRegistrationUseCase : ICustomerRegistrationUseCase
    {
        private readonly ICustomerRegisterEventPublisher _customerRegisterEventPublisher;

        public CustomerRegistrationUseCase(
            ICustomerRegisterEventPublisher customerRegisterEventPublisher
        )
        {
            _customerRegisterEventPublisher = customerRegisterEventPublisher;
        }

        public void Register(Customer customer)
        {
            _customerRegisterEventPublisher.Publish(customer);
            Console.WriteLine($"[Register] Customer registered: '{customer.Cpf}'; status: '{customer.RegisterStatus}'");

        }
    }
}