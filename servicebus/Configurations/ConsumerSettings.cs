namespace ServiceBus.Configurations
{
    public class ConsumerSettings
    {
        public string Topic { get; set; }
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public bool EnableAutoCommit { get; set; }
        public int AutoOffSetReset { get; set; }
        public bool EnablePartionEof { get; set; }
        public int MaxPollIntervalMs { get; set; }
        public string TopicRepublish { get; set; }
        public int Delay { get; set; }
    }
}