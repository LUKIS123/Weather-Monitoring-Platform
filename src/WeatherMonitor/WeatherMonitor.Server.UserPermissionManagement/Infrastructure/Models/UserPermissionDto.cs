namespace WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;

public readonly record struct UserPermissionDto(
    int Id,
    string UserId,
    int DeviceId);
