using ValidationService.Core.Entities;
using ValidationService.Core.UseCases;

namespace ValidationService.Core.Events.Processors
{
    public class CustomerRegistrationEventProcessor : ICustomerRegistrationEventProcessor
    {
        private readonly ICustomerValidationUseCase _customerValidationUseCase;

        public CustomerRegistrationEventProcessor(
            ICustomerValidationUseCase customerValidationUseCase
        )
        {
            _customerValidationUseCase = customerValidationUseCase;
        }

        public void Subscribe(Customer data)
        {
            _customerValidationUseCase.Validate(data);
        }
    }
}
