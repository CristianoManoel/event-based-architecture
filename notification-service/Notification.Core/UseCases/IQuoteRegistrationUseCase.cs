using Notification.Core.Entities;

namespace ClientService.Core.UseCases
{
    public interface IQuoteRegistrationUseCase
    {
        void Register(Quote quote);
    }
}