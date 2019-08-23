namespace ServiceBus
{
    public interface IEventProducer<T> : IEventProducer
    {
         void Produce(T data);
    }

    public interface IEventProducer
    {
        
    }
}