using Notification.Core.Entities;
using System;

namespace ClientService.Core.UseCases
{
    public class CustomerNotificationUseCase : ICustomerNotificationUseCase
    {
        public CustomerNotificationUseCase()
        {

        }

        public void Receive(Customer customer)
        {
            Console.WriteLine($"[Reveive Notification] '{customer.Cpf}'; status: '{customer.RegisterStatus}'");
        }
    }
}