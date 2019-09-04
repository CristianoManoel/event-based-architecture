using Microsoft.Extensions.DependencyInjection;
using WebTest.Infrastructure.Workers;

namespace WebTest.Infrastructure.Configuration
{
    public static class WorkersExtensions
    {
        public static IServiceCollection AddWorkers(this IServiceCollection serviceCollection)
        {
            // serviceCollection.AddHostedService<ChatSubscriber>();
            return serviceCollection;
        }
    }
}