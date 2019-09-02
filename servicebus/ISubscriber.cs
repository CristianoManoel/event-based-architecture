using ServiceBus.Configurations;
using ServiceBus.Events;
using System;
using System.Collections.Generic;

namespace ServiceBus.Kafka
{
    public interface ISubscriber
    {
        void SubscribeAsync<T>(ConsumerSettings settings, IEventProcessor<T> eventConsumer, Action<Exception> errorHandler = null);
    }
}