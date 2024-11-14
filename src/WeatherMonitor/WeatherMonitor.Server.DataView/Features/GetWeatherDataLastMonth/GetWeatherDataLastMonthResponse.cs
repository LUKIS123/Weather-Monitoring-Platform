using WeatherMonitor.Server.DataView.Infrastructure.Models;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastMonth;

internal readonly record struct GetWeatherDataLastMonthResponse(
    DateOnly StartDate,
    DateOnly EndDate,
    IEnumerable<LastMonthDailyData> DayTimeData,
    IEnumerable<LastMonthDailyData> NightTimeData);
