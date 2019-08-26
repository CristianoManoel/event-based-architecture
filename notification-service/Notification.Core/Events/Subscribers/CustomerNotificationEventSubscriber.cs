using ClientService.Core.UseCases;
using Notification.Core.Entities;
using ServiceBus;
using ServiceBus.Events;

namespace ClientService.Core.Events.Subscribers
{
    public interface ICustomerNotificationEventSubscriber : IEventSubscriber<Customer> { }

    public class CustomerNotificationEventSubscriber : ICustomerNotificationEventSubscriber
    {
        private readonly ICustomerNotificationUseCase _customerNotificationUseCase;

        public CustomerNotificationEventSubscriber(
            ICustomerNotificationUseCase customerNotificationUseCase
        )
        {
            _customerNotificationUseCase = customerNotificationUseCase;
        }

        public void Subscribe(Customer data, SubscribeEventArgs args)
        {
            _customerNotificationUseCase.Receive(data);
        }
    }
}
