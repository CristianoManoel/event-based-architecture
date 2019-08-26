using Notification.Core.Entities;
using ServiceBus;
using ServiceBus.Events;

namespace ClientService.Core.Events.Publishers
{
    public interface IQuoteEventPublisher : IEventPublisher<Quote> { };
}