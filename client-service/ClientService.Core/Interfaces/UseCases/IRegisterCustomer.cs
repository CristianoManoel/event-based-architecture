using ClientService.Core.Entities;

namespace ClientService.Core.Interfaces.UseCases
{
    public interface IRegisterCustomer
    {
         void Register(Customer customer);
    }
}