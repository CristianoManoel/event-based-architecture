using System;
using ValidationService.Core.Entities;
using ValidationService.Core.Events.Publishers;

namespace ValidationService.Core.UseCases
{
    public class CustomerValidationUseCase : ICustomerValidationUseCase
    {
        private readonly ICustomerValidationEventPublisher _eventPublisher;

        public CustomerValidationUseCase(
            ICustomerValidationEventPublisher eventPublisher
        )
        {
            _eventPublisher = eventPublisher;
        }

        public void Validate(Customer customer)
        {
            // TODO
            customer.RegisterStatus = Enums.RegisterStatus.Actived;
            _eventPublisher.Publish(customer);
            Console.WriteLine($"[Validate] Customer validated: CPF: '{customer.Cpf}'; status: '{customer.RegisterStatus}'");
        }
    }
}