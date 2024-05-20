using LoggerEnhancer.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggerEnhancer
{
    public static class ServiceRegistration
    {
        public static void AddLoggerEnhancer(this IServiceCollection services)
        {
            services.AddSingleton<IContext, Context>();
            services.AddSingleton(typeof(ILogger<>), typeof(Enhancer<>));
            services.AddSingleton(typeof(ILoggerEnhancer<>), typeof(Enhancer<>));
        }
    }
}

