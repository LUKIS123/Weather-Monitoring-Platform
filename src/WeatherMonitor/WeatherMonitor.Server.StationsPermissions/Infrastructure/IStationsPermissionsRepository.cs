using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

namespace WeatherMonitor.Server.StationsPermissions.Infrastructure;
public interface IStationsPermissionsRepository
{
    Task<(IEnumerable<AvailableStation> Stations, int totalItems)> GetAvailableStationsAsync(int pageNumber, int pageSize);
    Task<StationUserPermissionDto> GetStationPermissionStatusAsync(int stationId, string userId);
    Task SendPermissionRequestAsync(int stationId, string userId, PermissionStatus status, DateTime createdAt);
    Task<(IEnumerable<UsersPermissionRequestDto> requests, int totalItems)> GetPermissionRequestsAsync(
        int pageNumber, int pageSize, string userId);
}
