using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ValidationService.Infrastructure.Configurations;

namespace ValidationService.Infrastructure.Configuration
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