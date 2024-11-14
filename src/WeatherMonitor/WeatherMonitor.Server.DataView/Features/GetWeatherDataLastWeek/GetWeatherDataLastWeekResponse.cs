using WeatherMonitor.Server.DataView.Infrastructure.Models;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastWeek;
public readonly record struct GetWeatherDataLastWeekResponse(
    DateTime StartDateTime,
    DateTime EndDateTime,
    IEnumerable<LastWeekHourlyData> LastWeekHourlyData);