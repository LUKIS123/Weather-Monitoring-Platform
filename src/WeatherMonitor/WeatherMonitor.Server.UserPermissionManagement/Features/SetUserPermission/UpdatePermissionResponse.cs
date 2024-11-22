using WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.SetUserPermission;

internal readonly record struct UpdatePermissionResponse(
    UserPermissionRequestDto UserPermissionRequestDto,
    UserPermissionDto? UserPermissionDto
    );

