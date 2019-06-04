using ClientService.Core.Entities;
using ClientService.Core.Interfaces.Events.Publishers;
using ClientService.Infrastructure.Configurations;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace ClientService.Infrastructure.Kafka
{
    public class EventPublisher<T> : IEventPublisher<T>
    where T : class
    {
        private readonly KafkaOptions KafkaOptions;
        public EventPublisher(KafkaOptions KafkaOptions)
        {
            this.KafkaOptions = KafkaOptions;

        }
        public async void PublishEvent(string topicName, string key, T data)
        {
            var config = new ProducerConfig { BootstrapServers = KafkaOptions.Producer.BootstrapServers };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                var serializedData = JsonConvert.SerializeObject(data);
                var deliveryReport = await producer.ProduceAsync(topicName, new Message<string, string> { Key = key, Value = serializedData });
            }
        }
    }
}