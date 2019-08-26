using ClientService.Core.Entities;

namespace ClientService.Core.UseCases
{
    public interface ICustomerRegistrationUseCase
    {
         void Register(Customer customer);
    }
}