using WebTest.Infrastructure.Configuration;

namespace WebTest.Infrastructure.Configurations
{
    public class AppConfigurationOptions
    {
        public KafkaOptions Kafka { get; set; }
    }
}