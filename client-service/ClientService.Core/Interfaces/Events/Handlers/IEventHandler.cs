using System.Threading.Tasks;
using ClientService.Core.Interfaces.Events.Processors;

namespace ClientService.Core.Interfaces.Events.Handlers
{
    public interface IEventHandler<T>
    where T : class
    {
        Task ConsumeEvents(string topicName, IEventProcessor<T> eventProcessor);
    }
}