using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public static int Count;

        private readonly KafkaOptions KafkaOptions;
        public EventPublisher(KafkaOptions KafkaOptions)
        {
            this.KafkaOptions = KafkaOptions;

        }
        public async Task PublishEvent(string topicName, List<T> data)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = KafkaOptions.Producer.BootstrapServers,
                // EnableIdempotence = true
            };

            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                // Console.WriteLine($"Starting");

                foreach (var d in data)
                {
                    try
                    {
                        var key = d.GetType().GetProperty("Id").GetValue(d).ToString();
                        var serializedData = JsonConvert.SerializeObject(d);

                        var deliveryResult = await producer.ProduceAsync(topicName, new Message<string, string> { Key = key, Value = serializedData });
                        Console.WriteLine($"Delivered '{key}' to '{deliveryResult.TopicPartitionOffset}'");
                    }
                    catch (ProduceException<string, string> e)
                    {
                        throw new Exception($"Delivery failed: {e.Error.Reason}");
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Delivery failed: {e.Message}");
                    }
                }
            }
        }

    }
}
 