using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.StationsPermissions.Features.ListAvailableStations;

namespace WeatherMonitor.Server.StationsPermissions;

public static class StationsPermissionsModule
{
    public static IServiceCollection AddStationsPermissionsModule(this IServiceCollection services)
    {
        services.AddTransient<IListAvailableStationsService, ListAvailableStationsService>();

        return services;
    }
}
