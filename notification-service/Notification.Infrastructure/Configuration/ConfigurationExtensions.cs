using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Notification.Infrastructure.Configurations;

namespace Notification.Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<ConfigurationOptions>(configuration);
            return serviceCollection;
        }

    }
}