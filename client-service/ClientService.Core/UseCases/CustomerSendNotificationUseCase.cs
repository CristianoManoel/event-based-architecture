using ClientService.Core.Entities;
using ClientService.Core.Events.Publishers;
using ServiceBus;
using System;

namespace ClientService.Core.UseCases
{
    public class CustomerSendNotificationUseCase : ICustomerSendNotificationUseCase
    {
        private readonly ICustomerNotificationEventPublisher _customerNotificationEventPublisher;

        public CustomerSendNotificationUseCase(
            ICustomerNotificationEventPublisher customerNotificationEventPublisher
        )
        {
            _customerNotificationEventPublisher = customerNotificationEventPublisher;
        }

        public void Send(Customer customer)
        {
            _customerNotificationEventPublisher.Publish(customer);
            Console.WriteLine($"[Send Notification] Customer validated: '{customer.Cpf}'; status: '{customer.RegisterStatus}'");
        }
    }
}