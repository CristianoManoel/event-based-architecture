using ClientService.Core.Entities;
using ClientService.Core.Interfaces;
using ClientService.Infrastructure.Configurations;
using Confluent.Kafka;

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
            var config = new ProducerConfig { BootstrapServers = KafkaOptions.BootstrapServers };

            try{
            using (var producer = new ProducerBuilder<string, T>(config).Build())
            {
                var data1 = new Customer();
                var deliveryReport = await producer.ProduceAsync(topicName, new Message<string, T> { Key = key, Value = data });
            }
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}