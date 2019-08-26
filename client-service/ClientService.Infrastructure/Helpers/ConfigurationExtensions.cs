//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.CommandLine;

//namespace ClientService.Infrastructure.Helpers
//{
//    public static class ConfigurationExtensions
//    {
//        public static string[] GetCommandLineArgs(this IConfiguration config)
//        {
//            var cl = config.GetProvider<CommandLineConfigurationProvider>();
//            var args = (string[])cl.GetType().GetProperty("Args", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(config);
//            return args;
//        }

//        public static IConfigurationProvider GetCommandLineProvider(this IConfiguration config)
//        {
//            return config.GetProvider<CommandLineConfigurationProvider>();
//        }

//        public static T GetProvider<T>(this IConfiguration config) where T : class, IConfigurationProvider 
//        {
//            if (config is ConfigurationRoot configurationRoot)
//                return GetProvider<T>(configurationRoot);

//            return default;
//        }

//        public static T GetProvider<T>(this ConfigurationRoot configRoot) where T : IConfigurationProvider 
//        {
//            foreach (var p in configRoot.Providers)
//            {
//                if (p is T)
//                    return (T)p;
//            }

//            return default;
//        }
//    }
//}
