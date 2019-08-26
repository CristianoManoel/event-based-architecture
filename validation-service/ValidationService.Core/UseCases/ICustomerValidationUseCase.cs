using ValidationService.Core.Entities;

namespace ValidationService.Core.UseCases
{
    public interface ICustomerValidationUseCase
    {
        void Validate(Customer customer);
    }
}