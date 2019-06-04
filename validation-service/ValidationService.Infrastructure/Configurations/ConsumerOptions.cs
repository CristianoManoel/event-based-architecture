namespace ValidationService.Infrastructure.Configurations
{
    public class ConsumerOptions
    {
        public string BootstrapServers { get; set; }

        public string GroupId { get; set; }

        public bool EnableAutoCommit { get; set; }

        public int AutoOffSetReset { get; set; }

        public bool EnablePartionEof { get; set; }
    }
}