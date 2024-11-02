using WeatherMonitor.Server.DataView.Infrastructure.Models;

namespace WeatherMonitor.Server.DataView.Infrastructure;
public interface IWeatherStationsRepository
{
    Task<(IEnumerable<GetStationResponse> Stations, int totalItems)> GetStationsAsync(int pageSize, int pageNumber);
    Task SetStationsActiveAsync(IEnumerable<int> stationsIds);
}
