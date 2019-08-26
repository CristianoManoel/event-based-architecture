using ServiceBus.Configurations;
using ServiceBus.Events;
using System;
using System.Collections.Generic;

namespace ServiceBus.Kafka
{
    public interface ISubscriber
    {
        void SubscribeAsync<T>(ConsumerSettings settings, IEnumerable<IEventSubscriber<T>> eventConsumers, Action<Exception> errorHandler = null);
    }
}