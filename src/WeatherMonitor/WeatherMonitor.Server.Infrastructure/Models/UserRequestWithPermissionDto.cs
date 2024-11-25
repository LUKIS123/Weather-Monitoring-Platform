using WeatherMonitor.Server.SharedKernel.Models;

namespace WeatherMonitor.Server.Infrastructure.Models;

internal readonly record struct UserRequestWithPermissionDto(
    int Id,
    string UserId,
    int DeviceId,
    string DeviceName,
    string GoogleMapsPlusCode,
    PermissionStatus PermissionStatus,
    DateTime DateTime,
    int PermissionId,
    string PermissionUserId,
    int PermissionDeviceId);