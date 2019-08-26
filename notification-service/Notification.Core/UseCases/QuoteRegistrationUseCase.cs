using ClientService.Core.Events.Publishers;
using Notification.Core.Entities;
using System;

namespace ClientService.Core.UseCases
{
    public class QuoteRegistrationUseCase : IQuoteRegistrationUseCase
    {
        private readonly IQuoteEventPublisher _quoteEventPublisher;

        public QuoteRegistrationUseCase(IQuoteEventPublisher quoteEventPublisher)
        {
            _quoteEventPublisher = quoteEventPublisher;
        }

        public void Register(Quote quote)
        {
            _quoteEventPublisher.Publish(quote);
            Console.WriteLine($"[Quote] Registration: '{quote.QuoteName}'; status: '{quote.Value}'");
        }
    }
}