using WeatherMonitor.Server.SharedKernel.Models;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.SetUserPermission;
internal readonly record struct UpdatePermissionRequest(
    string UserId,
    int DeviceId,
    PermissionStatus Status);
