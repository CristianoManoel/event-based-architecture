using Confluent.Kafka;
using ServiceBus.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBus.Kafka
{
    public class KafkaProducerFluent<T>
    {
        private readonly List<string> _brokerList = new List<string>();

        private string _topicName;

        public KafkaProducerFluent()
        {

        }

        #region Fluent setters

        public KafkaProducerFluent<T> WithConfig(ProducerSettings config)
        {
            AddBroker(config.BootstrapServers);
            _topicName = config.Topic;
            return this;
        }

        public KafkaProducerFluent<T> AddBroker(params string[] brokers)
        {
            _brokerList.AddRange(Helper.ParseHosts(brokers));
            return this;
        }

        #endregion

        public async Task<DeliveryResult<string, string>> ProduceAsync(string key, T data, string topicName = null, Headers headers = null)
        {
            var serializedData = JsonConvert.SerializeObject(data);
            return await ProduceAsStringAsync(key, serializedData, topicName, headers);
        }

        public async Task<DeliveryResult<string, string>> ProduceAsStringAsync(string key, string data, string topicName = null, Headers headers = null)
        {
            if (_brokerList.Count == 0)
                throw new InvalidOperationException($"One broker must be added to build a consumer. Use the {nameof(AddBroker)} method to add a broker!");

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = string.Join(", ", _brokerList.ToArray()),
                // EnableIdempotence = true
            };

            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                return await producer.ProduceAsync(topicName ?? _topicName, new Message<string, string> { Key = key, Value = data, Headers = headers });
            }
        }
    }
}
