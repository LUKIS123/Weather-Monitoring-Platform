namespace WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

public readonly record struct UsersPermissionDto(
    int Id,
    string UserId,
    int DeviceId);
