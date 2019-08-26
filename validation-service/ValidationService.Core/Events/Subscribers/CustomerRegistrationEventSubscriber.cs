using ValidationService.Core.Entities;
using ValidationService.Core.Events.Publishers;
using System;
using ValidationService.Core.UseCases;
using ServiceBus.Events;

namespace ValidationService.Core.Events.Subscribers
{
    public class CustomerRegistrationEventSubscriber : ICustomerRegistrationEventSubscriber
    {
        private readonly ICustomerValidationUseCase _customerValidationUseCase;

        public CustomerRegistrationEventSubscriber(
            ICustomerValidationUseCase customerValidationUseCase
        )
        {
            _customerValidationUseCase = customerValidationUseCase;
        }

        public void Subscribe(Customer data, SubscribeEventArgs args)
        {
            _customerValidationUseCase.Validate(data);
        }
    }
}
