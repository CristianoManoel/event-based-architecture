using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ClientService.Infrastructure.Configurations;

namespace ClientService.Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddAppConfiguration(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<AppConfigurationOptions>(configuration);
            return serviceCollection;
        }

    }
}