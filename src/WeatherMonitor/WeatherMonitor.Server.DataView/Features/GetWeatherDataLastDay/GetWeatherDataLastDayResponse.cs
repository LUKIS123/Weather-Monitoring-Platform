using WeatherMonitor.Server.DataView.Infrastructure.Models;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastDay;

internal readonly record struct GetWeatherDataLastDayResponse(
    DateTime CurrentDate,
    IEnumerable<LastDayHourlyData> HourlyData);
