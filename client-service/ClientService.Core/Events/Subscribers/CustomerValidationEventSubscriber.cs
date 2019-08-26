using ClientService.Core.Entities;
using ClientService.Core.Events.Publishers;
using ClientService.Core.UseCases;
using ServiceBus.Events;
using System;

namespace ClientService.Core.Events.Subscribers
{
    public class CustomerValidationEventSubscriber : ICustomerValidationEventSubscriber
    {
        private readonly ICustomerSendNotificationUseCase _notificationCustomerUseCase;

        public CustomerValidationEventSubscriber(
            ICustomerSendNotificationUseCase notificationCustomerUseCase
        )
        {
            _notificationCustomerUseCase = notificationCustomerUseCase;
        }

        public void Subscribe(Customer data, SubscribeEventArgs args)
        {
            _notificationCustomerUseCase.Send(data);
        }
    }
}
