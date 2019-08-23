using ServiceBus.Configurations;
using Notification.Infrastructure.Helpers;
using System.Collections;
using System.Collections.Generic;

namespace Notification.Infrastructure.Configuration
{
    public class KafkaOptions
    {
        public ProducersOptions Producers { get; set; }
        public ConsumersOptions Consumers { get; set; }

        public class ProducersOptions : PropertyCollection<ProducerSettings>
        {
            public ProducerSettings Notification { get; set; }
            public ProducerSettings Quotes { get; set; }
        }

        public class ConsumersOptions : PropertyCollection<ConsumerSettings>
        {
            public ConsumerSettings Notification { get; set; }
            public ConsumerSettings Quotes { get; set; }
        }
    }

}
