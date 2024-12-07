using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using WeatherMonitorCore.DataCleanUp.Utils;

namespace WeatherMonitorCore.DataCleanUp;

public class CleanupFunction
{
    private readonly ILogger _logger;
    private readonly IDataCleanupRepository _repository;
    private readonly ITimeZoneProvider _timeZoneProvider;
    private readonly TimeProvider _timeProvider;

    public CleanupFunction(
        ILoggerFactory loggerFactory,
        IDataCleanupRepository repository,
        ITimeZoneProvider timeZoneProvider,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _timeZoneProvider = timeZoneProvider;
        _timeProvider = timeProvider;
        _logger = loggerFactory.CreateLogger<CleanupFunction>();
    }

    [FunctionName("CleanupOldWeatherData")]
    public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer)
    {
        _logger.LogInformation($"Cleanup job started at: {DateTime.Now}");

        try
        {
            var zoneAdjustedTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(
                _timeProvider.GetUtcNow().DateTime,
                _timeZoneProvider.GetTimeZoneInfo());

            var rowsAffected = await _repository.RemoveOldDataAsync(zoneAdjustedTimeStamp);

            _logger.LogInformation($"Deleted {rowsAffected} old records from database.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during data cleanup.");
        }
    }
}