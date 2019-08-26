using ClientService.Infrastructure.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace ClientService.Infrastructure.Configuration
{
    public static class WorkersExtensions
    {
        public static IServiceCollection AddWorkers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<CustomerValidationSubscriber>();
            return serviceCollection;
        }
    }
}