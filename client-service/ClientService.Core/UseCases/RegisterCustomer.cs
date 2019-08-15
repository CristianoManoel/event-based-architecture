using ClientService.Core.Entities;
using ClientService.Core.Interfaces.Events.Publishers;
using ClientService.Core.Interfaces.UseCases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientService.Core.UseCases
{
    public class RegisterCustomer : IRegisterCustomer
    {
        private readonly IEventPublisher<Customer> eventPublisher;
        
        public RegisterCustomer(IEventPublisher<Customer> eventStore)
        {
            this.eventPublisher = eventStore;
        }

        public async Task Register(List<Customer> customers)
        {
            await eventPublisher.PublishEvent(nameof(Customer), customers);
        }
    }
}