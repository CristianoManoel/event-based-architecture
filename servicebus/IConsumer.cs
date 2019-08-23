using System;
using System.Collections.Generic;
using Confluent.Kafka;
using ServiceBus.Configurations;

namespace ServiceBus
{
    public interface IConsumer
    {
        IConsumer AddBroker(params string[] brokers);
        IConsumer AutoOffSetReset(AutoOffsetReset autoOffSetReset);
        IConsumer AutoOffSetReset(int autoOffSetReset);
        IConsumer EnableAutoCommit(bool enabled);
        IConsumer EnablePartionEof(bool? enablePartionEof);
        IConsumer Error(Action<Exception> error);
        IConsumer MaxPollIntervalMs(int maxPollIntervalMs);
        IConsumer Success(IEnumerable<IEventConsumer> eventConsumers);
        IConsumer Success(IEventConsumer eventConsumer);
        IConsumer Success<T>(Action<T> success);
        IConsumer WithConfig(ConsumerSettings config);
        IConsumer WithGroupId(string groupId);
        void Subscribe(string topicName = null);
    }
}