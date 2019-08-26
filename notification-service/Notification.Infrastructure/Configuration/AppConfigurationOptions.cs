using Notification.Infrastructure.Configuration;

namespace Notification.Infrastructure.Configurations
{
    public class AppConfigurationOptions
    {
        public KafkaOptions Kafka { get; set; }
        public WorkersOptions Workers { get; set; }
    }
}