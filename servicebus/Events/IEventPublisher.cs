namespace ServiceBus.Events
{
    public interface IEventPublisher<T>
    {
         void Publish(T data);
    }
}