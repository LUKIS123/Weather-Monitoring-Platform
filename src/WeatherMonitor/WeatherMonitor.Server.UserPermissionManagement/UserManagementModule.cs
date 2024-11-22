using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.UserPermissionManagement.Features.GetPendingPermissionRequests;

namespace WeatherMonitor.Server.UserPermissionManagement;

public static class UserManagementModule
{
    public static IServiceCollection AddUserManagementModule(this IServiceCollection services)
    {
        services.AddTransient<IGetPendingRequestsService, GetPendingRequestsService>();

        return services;
    }
}
