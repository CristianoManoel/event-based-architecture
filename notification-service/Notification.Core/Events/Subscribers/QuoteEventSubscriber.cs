using ClientService.Core.UseCases;
using Notification.Core.Entities;
using ServiceBus;
using ServiceBus.Events;

namespace ClientService.Core.Events.Subscribers
{
    public interface IQuoteEventSubscriber : IEventSubscriber<Quote> { }

    public class QuoteEventSubscriber : IQuoteEventSubscriber
    {
        private readonly IQuoteReceiveUseCase _quoteReceiveUseCase;

        public QuoteEventSubscriber(
            IQuoteReceiveUseCase quoteReceiveUseCase
        )
        {
            _quoteReceiveUseCase = quoteReceiveUseCase;
        }

        public void Subscribe(Quote data, SubscribeEventArgs args)
        {
            _quoteReceiveUseCase.Receive(data);
        }
    }
}
