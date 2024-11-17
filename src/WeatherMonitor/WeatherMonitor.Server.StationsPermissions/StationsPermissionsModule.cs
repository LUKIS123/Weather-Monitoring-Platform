using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.StationsPermissions.Features.GetUsersPermissions;
using WeatherMonitor.Server.StationsPermissions.Features.ListAvailableStations;
using WeatherMonitor.Server.StationsPermissions.Features.SendPermissionRequest;
using WeatherMonitor.Server.StationsPermissions.Features.StationPermissionStatus;

namespace WeatherMonitor.Server.StationsPermissions;

public static class StationsPermissionsModule
{
    public static IServiceCollection AddStationsPermissionsModule(this IServiceCollection services)
    {
        services.AddTransient<IListAvailableStationsService, ListAvailableStationsService>();
        services.AddTransient<IGetStationPermissionStatusService, GetStationPermissionStatusService>();
        services.AddTransient<ISendPermissionRequestService, SendPermissionRequestService>();
        services.AddTransient<IGetUsersPermissionsService, GetUsersPermissionsService>();

        return services;
    }
}
