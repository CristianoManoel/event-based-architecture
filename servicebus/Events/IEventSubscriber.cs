namespace ServiceBus.Events
{
    public interface IEventSubscriber<T>
    {
        void Subscribe(T data, SubscribeEventArgs args);
    }
}