using ServiceBus;
using ServiceBus.Events;
using ValidationService.Core.Entities;

namespace ValidationService.Core.Events.Subscribers
{
    public interface ICustomerRegistrationEventSubscriber : IEventSubscriber<Customer> { }
}