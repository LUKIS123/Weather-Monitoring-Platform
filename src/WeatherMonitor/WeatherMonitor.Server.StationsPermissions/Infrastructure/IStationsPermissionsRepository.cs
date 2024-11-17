using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

namespace WeatherMonitor.Server.StationsPermissions.Infrastructure;
public interface IStationsPermissionsRepository
{
    Task<(IEnumerable<AvailableStation> Stations, int totalItems)> GetAvailableStationsAsync(int pageNumber, int pageSize);
}
