using WeatherMonitor.Server.DataView.Infrastructure.Models;

namespace WeatherMonitor.Server.DataView.Infrastructure;
public interface IDataViewRepository
{
    Task<LastDayWeatherData> GetLastDayWeatherDataAsync(DateTime currentTime, int? deviceId = null,
        string? plusCodeSearch = null);
    Task<LastWeekWeatherData> GetLastWeekWeatherDataAsync(DateTime currentTime, int? deviceId = null,
        string? plusCodeSearch = null);
    Task<LastMonthWeatherData> GetDayTimeLastMonthWeatherDataAsync(DateTime currentTime, int? deviceId = null,
        string? plusCodeSearch = null);
    Task<LastMonthWeatherData> GetNightTimeLastMonthWeatherDataAsync(DateTime currentTime, int? deviceId = null,
        string? plusCodeSearch = null);
}
