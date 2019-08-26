using ServiceBus.Configurations;

namespace Notification.Infrastructure.Configuration
{
    public class KafkaOptions
    {
        public ProducersOptions Producers { get; set; }
        public ConsumersOptions Consumers { get; set; }

        public class ProducersOptions
        {
            public ProducerSettings Quotes { get; set; }
        }

        public class ConsumersOptions
        {
            public ConsumerSettings CustomerNotification { get; set; }
            public ConsumerSettings Quotes { get; set; }
        }
    }

}
