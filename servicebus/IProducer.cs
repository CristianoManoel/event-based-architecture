using System;
using Confluent.Kafka;
using ServiceBus.Configurations;

namespace ServiceBus
{
    public interface IProducer
    {
        IProducer AddBroker(params string[] brokers);
        IProducer Error(Action<Exception> error);
        IProducer Success(Action success);
        IProducer WithConfig(ProducerSettings config);
        void Produce<T>(string key, T data, string topicName = null);
    }
}