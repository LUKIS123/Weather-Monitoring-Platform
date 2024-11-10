using Dapper;
using System.Text;
using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;

namespace WeatherMonitor.Server.Infrastructure.Repositories;
internal class DataViewRepository : IDataViewRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DataViewRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<LastDayWeatherData> GetLastDayWeatherDataAsync(DateTime currentTime, int? deviceId = null)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var sql = new StringBuilder(@$"
DECLARE @CurrentDateTime DATETIME = @currentTime;

WITH
    HourOffset AS (
        SELECT 0 AS HourOffset
        UNION ALL
        SELECT HourOffset + 1
        FROM HourOffset
        WHERE HourOffset < 23
    )
SELECT
    DATEADD(HOUR, -h.HourOffset, @CurrentDateTime) AS {nameof(LastDayHourlyData.HourlyTimeStamp)},
    ROUND(AVG(s.Temperature), 2) AS {nameof(LastDayHourlyData.AvgTemperature)},
    ROUND(AVG(s.Humidity), 2) AS {nameof(LastDayHourlyData.AvgHumidity)},
    ROUND(AVG(s.AirPressure), 2) AS {nameof(LastDayHourlyData.AvgAirPressure)},
    ROUND(AVG(s.PM1_0), 2) AS {nameof(LastDayHourlyData.AvgPM1_0)},
    ROUND(AVG(s.PM2_5), 2) AS {nameof(LastDayHourlyData.AvgPM2_5)},
    ROUND(AVG(s.PM10), 2) AS {nameof(LastDayHourlyData.AvgPM10)}
FROM
    HourOffset h
    LEFT JOIN [weatherData].[SensorsMeasurements] s
        ON DATEPART(HOUR, s.ReceivedAt) = DATEPART(HOUR, DATEADD(HOUR, -h.HourOffset, @CurrentDateTime))
        AND CAST(s.ReceivedAt AS DATE) = CAST(DATEADD(HOUR, -h.HourOffset, @CurrentDateTime) AS DATE)
        AND s.ReceivedAt >= DATEADD(HOUR, -24, @CurrentDateTime)
");
        if (deviceId.HasValue)
        {
            sql.Append(@"
WHERE s.DeviceId = @deviceId");
        }

        sql.Append($@"
GROUP BY
    DATEADD(HOUR, -h.HourOffset, @CurrentDateTime)
ORDER BY
    {nameof(LastDayHourlyData.HourlyTimeStamp)};
");

        var results = await connection.QueryAsync<LastDayHourlyData>(
            sql.ToString(),
            new
            {
                currentTime,
                deviceId
            });

        return new LastDayWeatherData(results);
    }
}
