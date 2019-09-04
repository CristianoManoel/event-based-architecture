using System;

namespace WebTest.Core.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        public DateTime SendDate { get; set; }
        public string Message { get; set; }
    }
}