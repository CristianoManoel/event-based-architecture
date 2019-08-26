using ValidationService.Core.Entities;
using ServiceBus;
using ServiceBus.Events;

namespace ValidationService.Core.Events.Publishers
{
    public interface ICustomerValidationEventPublisher : IEventPublisher<Customer> { };
}