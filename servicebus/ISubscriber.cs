using ServiceBus.Configurations;
using ServiceBus.Events;
using System;
using System.Threading;

namespace ServiceBus
{
    public interface ISubscriber
    {
        void SubscribeAsync<T>(ConsumerSettings settings, IEventProcessor<T> eventConsumer, Action<Exception> errorHandler = null, CancellationToken cancellationToken = default);
    }
}