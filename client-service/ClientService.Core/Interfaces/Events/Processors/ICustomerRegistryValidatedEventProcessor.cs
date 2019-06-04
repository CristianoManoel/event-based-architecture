using ClientService.Core.Entities;

namespace ClientService.Core.Interfaces.Events.Processors
{
    public interface ICustomerRegistryValidatedEventProcessor : IEventProcessor<Customer>
    {
    }
}