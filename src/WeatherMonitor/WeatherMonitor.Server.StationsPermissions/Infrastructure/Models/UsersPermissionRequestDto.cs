using WeatherMonitor.Server.SharedKernel.Models;

namespace WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

public readonly record struct UsersPermissionRequestDto(
    int Id,
    string UserId,
    int DeviceId,
    string DeviceName,
    PermissionStatus? PermissionStatus,
    DateTime? ChangeDate);
