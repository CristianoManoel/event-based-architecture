using ServiceBus.Configurations;

namespace WebTest.Infrastructure.Configuration
{
    public class KafkaOptions
    {
        public ProducersOptions Producers { get; set; }
        public ConsumersOptions Consumers { get; set; }

        public class ProducersOptions
        {
            public ProducerSettings Chat { get; set; }
        }

        public class ConsumersOptions
        {
            public ConsumerSettings Chat { get; set; }
        }
    }

}
