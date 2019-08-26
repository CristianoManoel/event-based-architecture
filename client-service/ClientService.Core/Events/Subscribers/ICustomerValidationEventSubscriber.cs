using ClientService.Core.Entities;
using ServiceBus;
using ServiceBus.Events;

namespace ClientService.Core.Events.Subscribers
{
    public interface ICustomerValidationEventSubscriber : IEventSubscriber<Customer> { }
}
