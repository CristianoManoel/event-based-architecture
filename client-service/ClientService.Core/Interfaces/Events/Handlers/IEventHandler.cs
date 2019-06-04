using ClientService.Core.Interfaces.Events.Processors;

namespace ClientService.Core.Interfaces.Events.Handlers
{
    public interface IEventHandler<T>
    where T : class
    {
        void ConsumeEvents(string topicName, IEventProcessor<T> eventProcessor);
    }
}