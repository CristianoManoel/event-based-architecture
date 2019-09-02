using ClientService.Core.Entities;
using ServiceBus;
using ServiceBus.Events;

namespace ClientService.Core.Events.Processors
{
    public interface ICustomerValidationEventProcessor : IEventProcessor<Customer> { }
}
