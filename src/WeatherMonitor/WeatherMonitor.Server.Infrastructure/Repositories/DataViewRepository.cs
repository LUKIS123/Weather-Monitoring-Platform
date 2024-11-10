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
        ON s.ReceivedAt >= DATEADD(HOUR, -h.HourOffset - 1, @CurrentDateTime)
            AND s.ReceivedAt < DATEADD(HOUR, -h.HourOffset, @CurrentDateTime)
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

    public async Task<LastWeekWeatherData> GetLastWeekWeatherDataAsync(DateTime currentTime, int? deviceId = null)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var sql = new StringBuilder(@$"
DECLARE @CurrentDateTime DATETIME = @currentTime;

WITH
    DateRange AS (
        SELECT CAST(DATEADD(DAY, -6, CAST(@CurrentDateTime AS DATE)) AS DATE) AS Date
        UNION ALL
        SELECT DATEADD(DAY, 1, Date)
        FROM DateRange
        WHERE Date < CAST(@CurrentDateTime AS DATE)
    ),
    HourRange AS (
        SELECT 0 AS HourOfDay
        UNION ALL
        SELECT HourOfDay + 1
        FROM HourRange
        WHERE HourOfDay < 23
    ),
    DateHourRange AS (
        SELECT
            DATEADD(HOUR, h.HourOfDay, CAST(d.Date AS DATETIME)) AS HourDateTime
        FROM DateRange d
        CROSS JOIN HourRange h
    )
SELECT
    dhr.HourDateTime AS {nameof(LastWeekHourlyData.HourDateTime)},
    ROUND(AVG(sm.Temperature), 2) AS {nameof(LastWeekHourlyData.AvgTemperature)},
    ROUND(AVG(sm.Humidity), 2) AS {nameof(LastWeekHourlyData.AvgHumidity)},
    ROUND(AVG(sm.AirPressure), 2) AS {nameof(LastWeekHourlyData.AvgAirPressure)},
    ROUND(AVG(sm.PM1_0), 2) AS {nameof(LastWeekHourlyData.AvgPM1_0)},
    ROUND(AVG(sm.PM2_5), 2) AS {nameof(LastWeekHourlyData.AvgPM2_5)},
    ROUND(AVG(sm.PM10), 2) AS {nameof(LastWeekHourlyData.AvgPM10)}
FROM
    DateHourRange dhr
    LEFT JOIN [weatherData].[SensorsMeasurements] sm
        ON sm.ReceivedAt >= dhr.HourDateTime
        AND sm.ReceivedAt < DATEADD(HOUR, 1, dhr.HourDateTime)
WHERE 
    (sm.ReceivedAt >= DATEADD(DAY, -7, @CurrentDateTime) OR sm.ReceivedAt IS NULL)
");

        if (deviceId.HasValue)
        {
            sql.Append(@"
AND sm.DeviceId = @deviceId");
        }

        sql.Append(@"
GROUP BY 
    dhr.HourDateTime
ORDER BY 
    dhr.HourDateTime
OPTION (MAXRECURSION 0);
");

        var results = await connection.QueryAsync<LastWeekHourlyData>(
            sql.ToString(),
            new
            {
                currentTime,
                deviceId
            });

        return new LastWeekWeatherData(results);
    }
}
