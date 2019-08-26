using ClientService.Core.Events.Publishers;
using Notification.Core.Entities;
using System;

namespace ClientService.Core.UseCases
{
    public class QuoteReceiveUseCase : IQuoteReceiveUseCase
    {
        public QuoteReceiveUseCase()
        {
        }

        public void Receive(Quote quote)
        {
            Console.WriteLine($"[Quote] Receive: '{quote.QuoteName}'; status: '{quote.Value}'");
        }
    }
}