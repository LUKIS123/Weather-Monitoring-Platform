using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;

public readonly record struct UserDto(
    string Id,
    Role Role,
    string Email,
    string Nickname);
