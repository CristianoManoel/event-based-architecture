using Microsoft.Extensions.DependencyInjection;
using Notification.Infrastructure.Workers;

namespace Notification.Infrastructure.Configuration
{
    public static class WorkersExtensions
    {
        public static IServiceCollection AddWorkers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<ConsumersWorker>();
            serviceCollection.AddHostedService<QuotesWorker>();
            return serviceCollection;
        }
    }
}