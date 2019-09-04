using ServiceBus.Configurations;
using System;
using System.Threading.Tasks;

namespace ServiceBus
{
    public interface IPublisher
    {
        Task PublishAsync<T>(ProducerSettings settings, string key, T data, Action success = null, Action<Exception> errorHandler = null);
    }
}