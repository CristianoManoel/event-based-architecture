//using ClientService.Core.Interfaces.Events.Publishers;
//using System;


//namespace ValidationService.Core.UseCases
//{
//    public class NotificationUseCase : ICustomerRegistryValidationUseCase
//    {
//        private readonly IEventPublisher<Customer> eventPublisher;
//        const string Topic = "Customer-Validation" ;
//        public CustomerRegistryValidationUseCase(IEventPublisher<Customer> eventPublisher)
//        {
//            this.eventPublisher = eventPublisher;
//        }
//        public void Validate(Customer customer)
//        {
//            customer.RegisterStatus = Enums.RegisterStatus.Actived;
//            Console.WriteLine($"{customer.Id} = New Customer:[{customer.Name}] registry validated.");
//            eventPublisher.PublishEvent(Topic, customer.Id.ToString(), customer);
//        }
//    }
//}