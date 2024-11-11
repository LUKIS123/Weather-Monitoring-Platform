namespace WeatherMonitor.Server.DataView.Infrastructure.Models;

public readonly record struct LastMonthWeatherData(
    IEnumerable<LastMonthDailyData> DailyData
);

public readonly record struct LastMonthDailyData(
    DateOnly Date,
    double? AvgTemperature,
    double? AvgHumidity,
    double? AvgAirPressure,
    double? AvgPM1_0,
    double? AvgPM2_5,
    double? AvgPM10);