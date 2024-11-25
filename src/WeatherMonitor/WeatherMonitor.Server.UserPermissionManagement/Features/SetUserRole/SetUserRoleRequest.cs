using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.SetUserRole;

internal readonly record struct SetUserRoleRequest(
    string UserId,
    Role Role);

