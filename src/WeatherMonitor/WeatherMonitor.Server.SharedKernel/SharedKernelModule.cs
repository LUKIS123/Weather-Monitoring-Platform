using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.SharedKernel.DIExtensions;

namespace WeatherMonitor.Server.SharedKernel;
public static class SharedKernelModule
{
    public static IServiceCollection AddSharedKernelModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClientProviders(configuration);
        return services;
    }
}
