using ServiceBus.Events;
using ValidationService.Core.Entities;

namespace ValidationService.Core.Events.Processors
{
    public interface ICustomerRegistrationEventProcessor : IEventProcessor<Customer> { }
}