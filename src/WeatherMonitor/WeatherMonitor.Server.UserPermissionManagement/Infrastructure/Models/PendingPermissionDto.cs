using WeatherMonitor.Server.SharedKernel.Models;

namespace WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;

public readonly record struct PendingPermissionDto(
    int Id,
    string UserId,
    int DeviceId,
    string DeviceName,
    string GoogleMapsPlusCode,
    PermissionStatus Status,
    DateTime RequestedAt,
    string Nickname,
    string Email);
