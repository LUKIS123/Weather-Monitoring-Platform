using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

namespace WeatherMonitor.Server.StationsPermissions.Features.StationPermissionStatus;

internal readonly record struct StationPermissionStatusResponse(
    StationUserPermissionDto StationUserPermission,
    bool HasPermission,
    bool CanRequestPermission
    );
