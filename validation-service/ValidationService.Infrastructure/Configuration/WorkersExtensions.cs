using ValidationService.Infrastructure.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace ValidationService.Infrastructure.Configuration
{
    public static class WorkersExtensions
    {
        public static IServiceCollection AddWorkers(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHostedService<CustomerRegistrationSubscriber>();
            return serviceCollection;
        }
    }
}