using Notification.Core.Entities;

namespace ClientService.Core.UseCases
{
    public interface IQuoteReceiveUseCase
    {
        void Receive(Quote quote);
    }
}