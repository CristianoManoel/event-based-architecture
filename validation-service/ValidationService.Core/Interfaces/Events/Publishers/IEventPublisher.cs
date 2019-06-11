namespace ValidationService.Core.Interfaces.Events.Publishers
{
    public interface IEventPublisher<T> 
    where T : class
    {
         void PublishEvent(string topicName, string key,  T data);
    }
}