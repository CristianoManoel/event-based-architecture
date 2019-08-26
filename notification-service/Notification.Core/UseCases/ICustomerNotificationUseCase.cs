using Notification.Core.Entities;

namespace ClientService.Core.UseCases
{
    public interface ICustomerNotificationUseCase
    {
        void Receive(Customer customer);
    }
}