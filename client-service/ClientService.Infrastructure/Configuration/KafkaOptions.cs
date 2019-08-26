using ServiceBus.Configurations;

namespace ClientService.Infrastructure.Configuration
{
    public class KafkaOptions
    {
        public ProducersOptions Producers { get; set; }
        public ConsumersOptions Consumers { get; set; }

        public class ProducersOptions
        {
            public ProducerSettings CustomerNotification { get; set; }
            public ProducerSettings Customer { get; set; }
        }

        public class ConsumersOptions
        {
            public ConsumerSettings CustomerValidation { get; set; }
        }
    }

}
