namespace ServiceBus.Events
{
    public interface IEventProcessor<T>
    {
        void Subscribe(T data);
    }
}