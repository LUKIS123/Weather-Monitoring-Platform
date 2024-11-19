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

    public async Task<LastDayWeatherData> GetLastDayWeatherDataAsync(DateTime currentTime, int? deviceId = null, string? plusCodeSearch = null)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var sql = new StringBuilder(@$"
DECLARE @CurrentDateTime DATETIME = @currentTime;

WITH HourOffset AS (
    SELECT 0 AS HourOffset
    UNION ALL
    SELECT HourOffset + 1
    FROM HourOffset
    WHERE HourOffset < 23
)
SELECT
    DATEADD(HOUR, -h.HourOffset, DATEADD(MINUTE, -DATEPART(MINUTE, @CurrentDateTime), @CurrentDateTime)) AS {nameof(LastDayHourlyData.HourlyTimeStamp)},
    ROUND(AVG(s.Temperature), 2) AS {nameof(LastDayHourlyData.AvgTemperature)},
    ROUND(AVG(s.Humidity), 2) AS {nameof(LastDayHourlyData.AvgHumidity)},
    ROUND(AVG(s.AirPressure), 2) AS {nameof(LastDayHourlyData.AvgAirPressure)},
    ROUND(AVG(s.PM1_0), 2) AS {nameof(LastDayHourlyData.AvgPM1_0)},
    ROUND(AVG(s.PM2_5), 2) AS {nameof(LastDayHourlyData.AvgPM2_5)},
    ROUND(AVG(s.PM10), 2) AS {nameof(LastDayHourlyData.AvgPM10)}
FROM
    HourOffset h
    LEFT JOIN [weatherData].[SensorsMeasurements] s
        ON s.ReceivedAt >= DATEADD(HOUR, -h.HourOffset - 1, DATEADD(MINUTE, -DATEPART(MINUTE, @CurrentDateTime), @CurrentDateTime))
           AND s.ReceivedAt < DATEADD(HOUR, -h.HourOffset, DATEADD(MINUTE, -DATEPART(MINUTE, @CurrentDateTime), @CurrentDateTime))
");

        if (deviceId.HasValue)
        {
            sql.Append(@"
WHERE s.DeviceId = @weatherDeviceId");
        }
        else if (!string.IsNullOrWhiteSpace(plusCodeSearch))
        {
            sql.Append(@"
    LEFT JOIN [identity].[Devices] d ON d.Id = s.DeviceId
WHERE CONTAINS(d.GoogleMapsPlusCode, @mapsPlusCodeSearch)");
        }

        sql.Append(@"
GROUP BY
    DATEADD(HOUR, -h.HourOffset, DATEADD(MINUTE, -DATEPART(MINUTE, @CurrentDateTime), @CurrentDateTime))
ORDER BY
    DATEADD(HOUR, -h.HourOffset, DATEADD(MINUTE, -DATEPART(MINUTE, @CurrentDateTime), @CurrentDateTime));
");

        var results = await connection.QueryAsync<LastDayHourlyData>(
            sql.ToString(),
            new
            {
                currentTime,
                weatherDeviceId = deviceId ?? (object)DBNull.Value,
                mapsPlusCodeSearch = string.IsNullOrWhiteSpace(plusCodeSearch)
                    ? (object)DBNull.Value
                    : plusCodeSearch
            });

        return new LastDayWeatherData(results);
    }

    //     public async Task<LastWeekWeatherData> GetLastWeekWeatherDataAsync(DateTime currentTime, int? deviceId = null,
    //         string? plusCodeSearch = null)
    //     {
    //         using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
    //         var sql = new StringBuilder(@$"
    // DECLARE @CurrentDateTime DATETIME = @currentTime;
    //
    // WITH
    //     Numbers AS (
    //         SELECT TOP (28)
    //             ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS Number
    //         FROM
    //             (VALUES (0),(0),(0),(0),(0),(0),(0)) AS a(n)
    //             CROSS JOIN (VALUES (0),(0),(0),(0)) AS b(n)
    //     ),
    //     DateHourRange AS (
    //         SELECT
    //             DATEADD(HOUR, -n.Number * 6, @CurrentDateTime) AS HourDateTime
    //         FROM Numbers n
    //     )
    // SELECT
    //     dhr.HourDateTime AS {nameof(LastWeekHourlyData.HourDateTime)},
    //     ROUND(AVG(sm.Temperature), 2) AS {nameof(LastWeekHourlyData.AvgTemperature)},
    //     ROUND(AVG(sm.Humidity), 2) AS {nameof(LastWeekHourlyData.AvgHumidity)},
    //     ROUND(AVG(sm.AirPressure), 2) AS {nameof(LastWeekHourlyData.AvgAirPressure)},
    //     ROUND(AVG(sm.PM1_0), 2) AS {nameof(LastWeekHourlyData.AvgPM1_0)},
    //     ROUND(AVG(sm.PM2_5), 2) AS {nameof(LastWeekHourlyData.AvgPM2_5)},
    //     ROUND(AVG(sm.PM10), 2) AS {nameof(LastWeekHourlyData.AvgPM10)}
    // FROM
    //     DateHourRange dhr
    //     LEFT JOIN [weatherData].[SensorsMeasurements] sm
    //         ON sm.ReceivedAt >= dhr.HourDateTime
    //            AND sm.ReceivedAt < DATEADD(HOUR, 6, dhr.HourDateTime)");
    //
    //         if (deviceId.HasValue)
    //         {
    //             sql.Append(@"
    // AND (sm.DeviceId = @deviceId)");
    //         }
    //
    //         sql.Append(@"
    // GROUP BY 
    //     dhr.HourDateTime
    // ORDER BY 
    //     dhr.HourDateTime;
    // ");
    //
    //         var results = await connection.QueryAsync<LastWeekHourlyData>(
    //             sql.ToString(),
    //             new
    //             {
    //                 currentTime,
    //                 deviceId
    //             });
    //
    //         return new LastWeekWeatherData(results);
    //     }

    public async Task<LastWeekWeatherData> GetLastWeekWeatherDataAsync(DateTime currentTime, int? deviceId = null, string? plusCodeSearch = null)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var sql = deviceId.HasValue
            ? BuildLastWeekDataSingleStationQuery()
            : BuildLastWeekDataAggregateQuery(plusCodeSearch);

        var results = await connection.QueryAsync<LastWeekHourlyData>(
            sql,
            new
            {
                currentTime,
                weatherDeviceId = deviceId ?? (object)DBNull.Value,
                mapsPlusCodeSearch = string.IsNullOrWhiteSpace(plusCodeSearch)
                    ? (object)DBNull.Value
                    : plusCodeSearch
            });

        return new LastWeekWeatherData(results);
    }

    private string BuildLastWeekDataAggregateQuery(string? plusCodeSearch)
    {
        var isSearchQuery = !string.IsNullOrWhiteSpace(plusCodeSearch);
        var sql = new StringBuilder(@$"
DECLARE @CurrentDateTime DATETIME = @currentTime;

DECLARE @AlignedDateTime DATETIME = DATEADD(HOUR, (DATEDIFF(HOUR, 0, @CurrentDateTime) / 6) * 6, 0);

WITH
    Numbers AS (
        SELECT TOP (28)
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS Number
        FROM
            (VALUES (0),(0),(0),(0),(0),(0),(0)) AS a(n)
            CROSS JOIN (VALUES (0),(0),(0),(0)) AS b(n)
    ),
    DateHourRange AS (
        SELECT
            DATEADD(HOUR, -n.Number * 6, @AlignedDateTime) AS HourDateTime
        FROM Numbers n
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
           AND sm.ReceivedAt < DATEADD(HOUR, 6, dhr.HourDateTime)");
        if (isSearchQuery)
        {
            sql.Append(@"
    LEFT JOIN [identity].[Devices] d ON d.Id = sm.DeviceId");
        }
        sql.Append(@"
WHERE
    dhr.HourDateTime <= @AlignedDateTime");
        if (isSearchQuery)
        {
            sql.Append(@"
AND CONTAINS(d.GoogleMapsPlusCode, @mapsPlusCodeSearch)");
        }

        sql.Append($@"
GROUP BY 
    dhr.HourDateTime

UNION ALL

SELECT
    @AlignedDateTime AS {nameof(LastWeekHourlyData.HourDateTime)},
    ROUND(AVG(sm.Temperature), 2) AS {nameof(LastWeekHourlyData.AvgTemperature)},
    ROUND(AVG(sm.Humidity), 2) AS {nameof(LastWeekHourlyData.AvgHumidity)},
    ROUND(AVG(sm.AirPressure), 2) AS {nameof(LastWeekHourlyData.AvgAirPressure)},
    ROUND(AVG(sm.PM1_0), 2) AS {nameof(LastWeekHourlyData.AvgPM1_0)},
    ROUND(AVG(sm.PM2_5), 2) AS {nameof(LastWeekHourlyData.AvgPM2_5)},
    ROUND(AVG(sm.PM10), 2) AS {nameof(LastWeekHourlyData.AvgPM10)}
FROM
    [weatherData].[SensorsMeasurements] sm");
        if (isSearchQuery)
        {
            sql.Append(@"
    LEFT JOIN [identity].[Devices] d ON d.Id = sm.DeviceId");
        }
        sql.Append(@"
WHERE
    sm.ReceivedAt >= @AlignedDateTime
    AND sm.ReceivedAt < @CurrentDateTime");

        if (isSearchQuery)
        {
            sql.Append(@"
    AND CONTAINS(d.GoogleMapsPlusCode, @mapsPlusCodeSearch)");
        }

        sql.Append(@"
ORDER BY
    dhr.HourDateTime;
");
        return sql.ToString();
    }

    private string BuildLastWeekDataSingleStationQuery()
    {
        const string sql = @$"
DECLARE @CurrentDateTime DATETIME = @currentTime;

DECLARE @AlignedDateTime DATETIME = DATEADD(HOUR, (DATEDIFF(HOUR, 0, @CurrentDateTime) / 6) * 6, 0);

WITH
    Numbers AS (
        SELECT TOP (28)
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS Number
        FROM
            (VALUES (0),(0),(0),(0),(0),(0),(0)) AS a(n)
            CROSS JOIN (VALUES (0),(0),(0),(0)) AS b(n)
    ),
    DateHourRange AS (
        SELECT
            DATEADD(HOUR, -n.Number * 6, @AlignedDateTime) AS HourDateTime
        FROM Numbers n
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
           AND sm.ReceivedAt < DATEADD(HOUR, 6, dhr.HourDateTime)
           AND (sm.DeviceId = @weatherDeviceId)
WHERE
    dhr.HourDateTime <= @AlignedDateTime
GROUP BY 
    dhr.HourDateTime

UNION ALL

SELECT
    @AlignedDateTime AS {nameof(LastWeekHourlyData.HourDateTime)},
    ROUND(AVG(sm.Temperature), 2) AS {nameof(LastWeekHourlyData.AvgTemperature)},
    ROUND(AVG(sm.Humidity), 2) AS {nameof(LastWeekHourlyData.AvgHumidity)},
    ROUND(AVG(sm.AirPressure), 2) AS {nameof(LastWeekHourlyData.AvgAirPressure)},
    ROUND(AVG(sm.PM1_0), 2) AS {nameof(LastWeekHourlyData.AvgPM1_0)},
    ROUND(AVG(sm.PM2_5), 2) AS {nameof(LastWeekHourlyData.AvgPM2_5)},
    ROUND(AVG(sm.PM10), 2) AS {nameof(LastWeekHourlyData.AvgPM10)}
FROM
    [weatherData].[SensorsMeasurements] sm
WHERE
    sm.ReceivedAt >= @AlignedDateTime
    AND sm.ReceivedAt < @CurrentDateTime
    AND sm.DeviceId = @weatherDeviceId

ORDER BY
    dhr.HourDateTime;
";
        return sql;
    }

    public async Task<LastMonthWeatherData> GetDayTimeLastMonthWeatherDataAsync(DateTime currentTime,
        int? deviceId = null, string? plusCodeSearch = null)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var sql = new StringBuilder(@$"
DECLARE @CurrentDateTime DATETIME = @currentTime;

WITH
    Numbers AS (
        SELECT TOP 30
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS Number
        FROM
            (VALUES (0),(0),(0),(0),(0),(0),(0),(0),(0),(0)) AS a(n)
            CROSS JOIN (VALUES (0),(0),(0)) AS b(n)
    ),
    DateRange AS (
        SELECT
            CAST(DATEADD(DAY, -n.Number, CAST(@CurrentDateTime AS DATE)) AS DATE) AS Date
        FROM Numbers n
    )
SELECT
    dr.Date AS {nameof(LastMonthDailyData.Date)},
    ROUND(AVG(sm.Temperature), 2) AS {nameof(LastMonthDailyData.AvgTemperature)},
    ROUND(AVG(sm.Humidity), 2) AS {nameof(LastMonthDailyData.AvgHumidity)},
    ROUND(AVG(sm.AirPressure), 2) AS {nameof(LastMonthDailyData.AvgAirPressure)},
    ROUND(AVG(sm.PM1_0), 2) AS {nameof(LastMonthDailyData.AvgPM1_0)},
    ROUND(AVG(sm.PM2_5), 2) AS {nameof(LastMonthDailyData.AvgPM2_5)},
    ROUND(AVG(sm.PM10), 2) AS {nameof(LastMonthDailyData.AvgPM10)}
FROM
    DateRange dr
    LEFT JOIN [weatherData].[SensorsMeasurements] sm
        ON CAST(sm.ReceivedAt AS DATE) = dr.Date
           AND DATEPART(HOUR, sm.ReceivedAt) BETWEEN 6 AND 17
");

        if (deviceId.HasValue)
        {
            sql.Append(@"
AND (sm.DeviceId = @weatherDeviceId)");
        }
        else if (!string.IsNullOrWhiteSpace(plusCodeSearch))
        {
            sql.Append(@"
    LEFT JOIN [identity].[Devices] d ON d.Id = sm.DeviceId
WHERE CONTAINS(d.GoogleMapsPlusCode, @mapsPlusCodeSearch)");
        }

        sql.Append(@"
GROUP BY 
    dr.Date
ORDER BY 
    dr.Date;
");
        var dayTimeResults = await connection.QueryAsync<LastMonthDailyData>(
            sql.ToString(),
            new
            {
                currentTime,
                weatherDeviceId = deviceId ?? (object)DBNull.Value,
                mapsPlusCodeSearch = string.IsNullOrWhiteSpace(plusCodeSearch)
                    ? (object)DBNull.Value
                    : plusCodeSearch
            });

        return new LastMonthWeatherData(dayTimeResults);
    }

    public async Task<LastMonthWeatherData> GetNightTimeLastMonthWeatherDataAsync(DateTime currentTime,
        int? deviceId = null, string? plusCodeSearch = null)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var sql = new StringBuilder(@$"
DECLARE @CurrentDateTime DATETIME = @currentTime;

WITH
    Numbers AS (
        SELECT TOP 30
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS Number
        FROM
            (VALUES (0),(0),(0),(0),(0),(0),(0),(0),(0),(0)) AS a(n)
            CROSS JOIN (VALUES (0),(0),(0)) AS b(n)
    ),
    DateRange AS (
        SELECT
            CAST(DATEADD(DAY, -n.Number, CAST(@CurrentDateTime AS DATE)) AS DATE) AS Date
        FROM Numbers n
    )
SELECT
    dr.Date AS {nameof(LastMonthDailyData.Date)},
    ROUND(AVG(sm.Temperature), 2) AS {nameof(LastMonthDailyData.AvgTemperature)},
    ROUND(AVG(sm.Humidity), 2) AS {nameof(LastMonthDailyData.AvgHumidity)},
    ROUND(AVG(sm.AirPressure), 2) AS {nameof(LastMonthDailyData.AvgAirPressure)},
    ROUND(AVG(sm.PM1_0), 2) AS {nameof(LastMonthDailyData.AvgPM1_0)},
    ROUND(AVG(sm.PM2_5), 2) AS {nameof(LastMonthDailyData.AvgPM2_5)},
    ROUND(AVG(sm.PM10), 2) AS {nameof(LastMonthDailyData.AvgPM10)}
FROM
    DateRange dr
    LEFT JOIN [weatherData].[SensorsMeasurements] sm
        ON CAST(sm.ReceivedAt AS DATE) = dr.Date
           AND (DATEPART(HOUR, sm.ReceivedAt) >= 18 OR DATEPART(HOUR, sm.ReceivedAt) < 6)
");

        if (deviceId.HasValue)
        {
            sql.Append(@"
AND (sm.DeviceId = @weatherDeviceId)");
        }
        else if (!string.IsNullOrWhiteSpace(plusCodeSearch))
        {
            sql.Append(@"
    LEFT JOIN [identity].[Devices] d ON d.Id = sm.DeviceId
WHERE CONTAINS(d.GoogleMapsPlusCode, @mapsPlusCodeSearch)");
        }

        sql.Append(@"
GROUP BY 
    dr.Date
ORDER BY 
    dr.Date;
");
        var nightTimeResults = await connection.QueryAsync<LastMonthDailyData>(
            sql.ToString(),
            new
            {
                currentTime,
                weatherDeviceId = deviceId ?? (object)DBNull.Value,
                mapsPlusCodeSearch = string.IsNullOrWhiteSpace(plusCodeSearch)
                    ? (object)DBNull.Value
                    : plusCodeSearch
            });

        return new LastMonthWeatherData(nightTimeResults);
    }
}
