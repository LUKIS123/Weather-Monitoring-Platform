using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

public readonly record struct StationUserPermissionDto(
    int StationId,
    string StationName,
    string UserId,
    Role UserRole,
    PermissionStatus? PermissionStatus,
    DateTime? ChangeDate,
    bool PermissionRecordExists
    );
