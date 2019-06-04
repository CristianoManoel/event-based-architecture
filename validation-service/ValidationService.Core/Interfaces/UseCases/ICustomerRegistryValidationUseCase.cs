using ValidationService.Core.Entities;

namespace ValidationService.Core.Interfaces.UseCases
{
    public interface ICustomerRegistryValidationUseCase
    {
         void Validate(Customer customer);
    }
}