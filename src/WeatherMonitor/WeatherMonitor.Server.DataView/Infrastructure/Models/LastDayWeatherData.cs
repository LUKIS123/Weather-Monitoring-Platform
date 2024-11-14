namespace WeatherMonitor.Server.DataView.Infrastructure.Models;

public readonly record struct LastDayWeatherData(
    IEnumerable<LastDayHourlyData> HourlyData
);

public readonly record struct LastDayHourlyData(
    DateTime HourlyTimeStamp,
    double? AvgTemperature,
    double? AvgHumidity,
    double? AvgAirPressure,
    double? AvgPM1_0,
    double? AvgPM2_5,
    double? AvgPM10);