using ClientService.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientService.Core.Interfaces.UseCases
{
    public interface IRegisterCustomer
    {
         Task Register(List<Customer> customer);
    }
}