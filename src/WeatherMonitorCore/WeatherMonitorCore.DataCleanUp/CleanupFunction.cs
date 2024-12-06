using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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

        var zoneAdjustedTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(
            TimeProvider.System.GetUtcNow().DateTime,
            _timeZoneProvider.GetTimeZoneInfo());

        var rowsAffected = await _repository.RemoveOldDataAsync(zoneAdjustedTimeStamp);

        _logger.LogInformation($"Deleted {rowsAffected} old records from database.");
    }
}