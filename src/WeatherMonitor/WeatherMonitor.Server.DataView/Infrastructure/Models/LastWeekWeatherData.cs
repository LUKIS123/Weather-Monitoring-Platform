namespace WeatherMonitor.Server.DataView.Infrastructure.Models;

public readonly record struct LastWeekWeatherData(
    IEnumerable<LastWeekHourlyData> LastWeekHourlyData);

public readonly record struct LastWeekHourlyData(
    DateTime HourDateTime,
    double? AvgTemperature,
    double? AvgHumidity,
    double? AvgAirPressure,
    double? AvgPM1_0,
    double? AvgPM2_5,
    double? AvgPM10);
