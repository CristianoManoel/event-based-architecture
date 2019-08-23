namespace Notification.Infrastructure.Configuration
{
    public class WorkersOptions
    {
        public QuoteOptions Quotes { get; set; }

        public class QuoteOptions
        {
            public string Url { get; set; }
            public int Time { get; set; }
        }
    }

}
