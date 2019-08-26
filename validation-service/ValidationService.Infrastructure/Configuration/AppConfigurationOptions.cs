using ValidationService.Infrastructure.Configuration;

namespace ValidationService.Infrastructure.Configurations
{
    public class AppConfigurationOptions
    {
        public KafkaOptions Kafka { get; set; }
        public WorkersOptions Workers { get; set; }
    }
}