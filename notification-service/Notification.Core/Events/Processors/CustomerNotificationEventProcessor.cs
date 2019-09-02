using ClientService.Core.UseCases;
using Notification.Core.Entities;
using ServiceBus;
using ServiceBus.Events;

namespace ClientService.Core.Events.Processors
{
    public interface ICustomerNotificationEventProcessor : IEventProcessor<Customer> { }

    public class CustomerNotificationEventProcessor : ICustomerNotificationEventProcessor
    {
        private readonly ICustomerNotificationUseCase _customerNotificationUseCase;

        public CustomerNotificationEventProcessor(
            ICustomerNotificationUseCase customerNotificationUseCase
        )
        {
            _customerNotificationUseCase = customerNotificationUseCase;
        }

        public void Subscribe(Customer data)
        {
            _customerNotificationUseCase.Receive(data);
        }
    }
}
