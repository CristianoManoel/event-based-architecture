using System;

namespace Notification.Core.Entities
{
    public class Quote
    {
        public DateTime Date { get; set; }
        public string QuoteName { get; set; }
        public decimal Value { get; set; }
    }
}
