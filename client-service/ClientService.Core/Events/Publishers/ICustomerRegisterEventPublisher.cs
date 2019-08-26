using ClientService.Core.Entities;
using ServiceBus;
using ServiceBus.Events;

namespace ClientService.Core.Events.Publishers
{
    public interface ICustomerRegisterEventPublisher : IEventPublisher<Customer> { };
}