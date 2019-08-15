using System;
using ValidationService.Core.Entities;
using ValidationService.Core.Interfaces.Events.Processors;
using ValidationService.Core.Interfaces.UseCases;

namespace ValidationService.Core.Events.Processors
{
    public class NewCustomerRegistryEventProcessor : INewCustomerRegistryEventProcessor<Customer>
    {
        private readonly ICustomerRegistryValidationUseCase customerRegistryValidationUseCase;
        public NewCustomerRegistryEventProcessor(ICustomerRegistryValidationUseCase customerRegistryValidationUseCase)
        {
            this.customerRegistryValidationUseCase = customerRegistryValidationUseCase;
        }

        public void Process(Customer data)
        {
            customerRegistryValidationUseCase.Validate(data);
        }
    }
}