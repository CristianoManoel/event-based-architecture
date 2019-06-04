using ClientService.Core.Entities;
using ClientService.Core.Interfaces.Events.Publishers;
using ClientService.Core.Interfaces.UseCases;

namespace ClientService.Core.UseCases
{
    public class RegisterCustomer : IRegisterCustomer
    {
        private readonly IEventPublisher<Customer> eventPublisher;
        public RegisterCustomer(IEventPublisher<Customer> eventStore)
        {
            this.eventPublisher = eventStore;

        }
        public void Register(Customer customer)
        {
            eventPublisher.PublishEvent(nameof(Customer), customer.Id.ToString(), customer);
        }
    }
}