using Confluent.Kafka;

namespace ServiceBus
{
    public interface IEventConsumer<T> : IEventConsumer
    {
        void Consume(T data);
    }

    public interface IEventConsumer
    {
    }
}