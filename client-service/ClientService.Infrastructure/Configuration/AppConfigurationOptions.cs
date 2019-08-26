using ClientService.Infrastructure.Configuration;

namespace ClientService.Infrastructure.Configurations
{
    public class AppConfigurationOptions
    {
        public KafkaOptions Kafka { get; set; }
        public WorkersOptions Workers { get; set; }
    }
}