using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientService.Core.Interfaces.Events.Publishers
{
    public interface IEventPublisher<T> 
    where T : class
    {
         Task PublishEvent(string topicName, List<T> data);
    }
}