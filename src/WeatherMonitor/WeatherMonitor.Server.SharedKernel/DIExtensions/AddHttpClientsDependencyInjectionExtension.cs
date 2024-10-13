using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.SharedKernel.Utility;

namespace WeatherMonitor.Server.SharedKernel.DIExtensions;
public static class AddHttpClientsDependencyInjectionExtension
{
    private const string CoreMicroserviceConfigurationSection = "CoreMicroservice";
    private const string BaseUrl = "BaseUrl";

    public static void AddHttpClientProviders(this IServiceCollection services, IConfiguration configuration)
    {
        var agileServiceBaseUri = new Uri(configuration[$"{CoreMicroserviceConfigurationSection}:{BaseUrl}"] ?? throw
            new ArgumentNullException(nameof(configuration), BaseUrl));

        services.AddHttpClient(
            Constants.CoreMicroserviceClient,
            c => c.BaseAddress = agileServiceBaseUri);
    }
}
