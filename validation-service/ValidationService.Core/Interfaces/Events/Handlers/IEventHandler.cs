using ValidationService.Core.Interfaces.Events.Processors;

namespace ValidationService.Core.Interfaces.Events.Handlers
{
    public interface IEventHandler<T>
    where T : class
    {
        void ConsumeEvents(string topicName, IEventProcessor<T> eventProcessor);
    }
}