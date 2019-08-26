using ServiceBus.Configurations;
using ServiceBus.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBus.Kafka
{
    public class KafkaServiceBus : ISubscriber, IPublisher
    {
        public KafkaServiceBus()
        {
            
        }

        public async void SubscribeAsync<T>(ConsumerSettings settings, IEnumerable<IEventSubscriber<T>> eventConsumers, Action<Exception> errorHandler = null)
        {
            await Task.Run(() =>
            {
                new KafkaConsumerFluent<T>()
                    .Success(eventConsumers)
                    .WithConfig(settings)
                    .Error(errorHandler)
                    .Subscribe();
            });
        }

        public async void PublishAsync<T>(ProducerSettings settings, string key, T data, Action success = null, Action<Exception> errorHandler = null)
        {
            await Task.Run(() =>
            {
                new KafkaProducerFluent<T>()
                    .WithConfig(settings)
                    .ProduceAsync(key, data);
            });
        }
    }
}
