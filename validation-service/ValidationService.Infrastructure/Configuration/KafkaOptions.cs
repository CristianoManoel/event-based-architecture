using ServiceBus.Configurations;

namespace ValidationService.Infrastructure.Configuration
{
    public class KafkaOptions
    {
        public ProducersOptions Producers { get; set; }
        public ConsumersOptions Consumers { get; set; }

        public class ProducersOptions
        {
            public ProducerSettings CustomerValidation { get; set; }
        }

        public class ConsumersOptions
        {
            public ConsumerSettings Customer { get; set; }
        }
    }

}
