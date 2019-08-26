using ServiceBus.Configurations;
using System;
using System.Collections.Generic;

namespace ServiceBus.Kafka
{
    public interface IPublisher
    {
        void PublishAsync<T>(ProducerSettings settings, string key, T data, Action success = null, Action<Exception> errorHandler = null);
    }
}