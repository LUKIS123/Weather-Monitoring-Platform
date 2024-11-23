using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.UserPermissionManagement.Features.GetPendingPermissionRequests;
using WeatherMonitor.Server.UserPermissionManagement.Features.GetUserPermissions;
using WeatherMonitor.Server.UserPermissionManagement.Features.GetUsers;
using WeatherMonitor.Server.UserPermissionManagement.Features.SetUserPermission;

namespace WeatherMonitor.Server.UserPermissionManagement;

public static class UserManagementModule
{
    public static IServiceCollection AddUserManagementModule(this IServiceCollection services)
    {
        services.AddTransient<IGetPendingRequestsService, GetPendingRequestsService>();
        services.AddTransient<ISetUsersStationPermissionService, SetUsersStationPermissionService>();
        services.AddTransient<IGetUsersService, GetUsersService>();
        services.AddTransient<IGetUsersPermissionsService, GetUsersPermissionsService>();

        return services;
    }
}
