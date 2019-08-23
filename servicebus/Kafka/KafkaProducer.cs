using Confluent.Kafka;
using ServiceBus.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServiceBus.Kafka
{
    public class KafkaProducer : IProducer
    {
        private readonly List<string> _brokerList = new List<string>();

        private string _topicName;
        private Action _success;
        private Action<Exception> _error;

        public KafkaProducer()
        {

        }

        #region Fluent setters

        public IProducer WithConfig(ProducerSettings config)
        {
            AddBroker(config.BootstrapServers);
            _topicName = config.Topic;
            return this;
        }

        public IProducer AddBroker(params string[] brokers)
        {
            _brokerList.AddRange(Helper.ParseHosts(brokers));
            return this;
        }

        public IProducer Success(Action success)
        {
            this._success = success;
            return this;
        }

        public IProducer Error(Action<Exception> error)
        {
            this._error = error;
            return this;
        }

        #endregion

        public async void Produce<T>(string key, T data, string topicName = null)
        {
            if (_brokerList.Count == 0)
                throw new InvalidOperationException($"One broker must be added to build a consumer. Use the {nameof(AddBroker)} method to add a broker!");

            var producerConfig = new Confluent.Kafka.ProducerConfig
            {
                BootstrapServers = string.Join(", ", _brokerList.ToArray()),
                // EnableIdempotence = true
            };

            using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
            {
                try
                {
                    var serializedData = JsonConvert.SerializeObject(data);
                    var dr = await producer.ProduceAsync(topicName ?? _topicName, new Message<string, string> { Key = key, Value = serializedData });
                    _success?.Invoke();
                }
                catch (ProduceException<string, string> e)
                {
                    _error?.Invoke(e);
                }
                catch (Exception e)
                {
                    _error?.Invoke(e);
                }
            }
        }
    }
}
