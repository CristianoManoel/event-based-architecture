using ClientService.Core.UseCases;
using Notification.Core.Entities;
using ServiceBus.Events;

namespace ClientService.Core.Events.Processors
{
    public interface IQuoteEventProcessor : IEventProcessor<Quote> { }

    public class QuoteEventProcessor : IQuoteEventProcessor
    {
        private readonly IQuoteReceiveUseCase _quoteReceiveUseCase;

        public QuoteEventProcessor(
            IQuoteReceiveUseCase quoteReceiveUseCase
        )
        {
            _quoteReceiveUseCase = quoteReceiveUseCase;
        }

        public void Subscribe(Quote data)
        {
            _quoteReceiveUseCase.Receive(data);
        }
    }
}
