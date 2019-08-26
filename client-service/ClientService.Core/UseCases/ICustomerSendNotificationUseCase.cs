using ClientService.Core.Entities;

namespace ClientService.Core.UseCases
{
    public interface ICustomerSendNotificationUseCase
    {
         void Send(Customer customer);
    }
}