using ClientService.Core.Entities;
using ClientService.Core.UseCases;

namespace ClientService.Core.Events.Processors
{
    public class CustomerValidationEventProcessor : ICustomerValidationEventProcessor
    {
        private readonly ICustomerSendNotificationUseCase _notificationCustomerUseCase;

        public CustomerValidationEventProcessor(
            ICustomerSendNotificationUseCase notificationCustomerUseCase
        )
        {
            _notificationCustomerUseCase = notificationCustomerUseCase;
        }

        public void Subscribe(Customer data)
        {
            _notificationCustomerUseCase.Send(data);
        }
    }
}
