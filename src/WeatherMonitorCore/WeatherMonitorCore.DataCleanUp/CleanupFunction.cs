using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using WeatherMonitorCore.DataCleanUp.Utils;

namespace WeatherMonitorCore.DataCleanUp;

public class CleanupFunction
{
    private readonly ILogger _logger;
    private readonly DataCleanupRepository _repository;
    private readonly TimeZoneProvider _timeZoneProvider;

    public CleanupFunction(
        ILoggerFactory loggerFactory,
        DataCleanupRepository repository,
        TimeZoneProvider timeZoneProvider)
    {
        _repository = repository;
        _timeZoneProvider = timeZoneProvider;
        _logger = loggerFactory.CreateLogger<CleanupFunction>();
    }

    [FunctionName("CleanupOldWeatherData")]
    public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer)
    {
        _logger.LogInformation($"Cleanup job started at: {DateTime.Now}");

        var zoneAdjustedTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(TimeProvider.System.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());



        var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        if (string.IsNullOrEmpty(connectionString))
        {
            _logger.LogError("Connection string not found in environment variables.");
            return;
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        const string sql = @"
DELETE FROM [weatherData].[SensorsMeasurements]
WHERE ReceivedAt < DATEADD(DAY, -31, GETDATE());
";

        await using var command = new SqlCommand(sql, connection);
        var rowsAffected = command.ExecuteNonQueryAsync();

        _logger.LogInformation($"Deleted {rowsAffected} old records from database.");

    }
}