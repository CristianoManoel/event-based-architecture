using ClientService.Core.Entities;
using ServiceBus.Events;

namespace ClientService.Core.Events.Publishers
{
    public interface ICustomerNotificationEventPublisher : IEventPublisher<Customer> { };
}