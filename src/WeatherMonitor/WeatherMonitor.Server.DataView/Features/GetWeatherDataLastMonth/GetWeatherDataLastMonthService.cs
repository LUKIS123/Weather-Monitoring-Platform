using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastMonth;

internal interface IGetWeatherDataLastMonthService
{
    Task<Result<GetWeatherDataLastMonthResponse>> Handle(int? deviceId);
}

internal class GetWeatherDataLastMonthService : IGetWeatherDataLastMonthService
{
    private readonly ITimeZoneProvider _timeZoneProvider;
    private readonly TimeProvider _timeProvider;
    private readonly IDataViewRepository _dataViewRepository;

    public GetWeatherDataLastMonthService(
        ITimeZoneProvider timeZoneProvider,
        TimeProvider timeProvider,
        IDataViewRepository dataViewRepository)
    {
        _timeZoneProvider = timeZoneProvider;
        _timeProvider = timeProvider;
        _dataViewRepository = dataViewRepository;
    }

    public async Task<Result<GetWeatherDataLastMonthResponse>> Handle(int? deviceId)
    {
        var zoneAdjustedTime =
            TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());
        zoneAdjustedTime = new DateTime(
            zoneAdjustedTime.Year,
            zoneAdjustedTime.Month,
            zoneAdjustedTime.Day,
            zoneAdjustedTime.Hour,
            0,
            0);

        var dayTimeResults = _dataViewRepository.GetDayTimeLastMonthWeatherDataAsync(zoneAdjustedTime, deviceId);
        var nightTimeResults = _dataViewRepository.GetNightTimeLastMonthWeatherDataAsync(zoneAdjustedTime, deviceId);
        await Task.WhenAll(dayTimeResults, nightTimeResults);

        var dayTimeData = dayTimeResults.Result.DailyData.ToArray();
        var nightTimeData = nightTimeResults.Result.DailyData.ToArray();

        var startDateTime = GetStartDateTime(zoneAdjustedTime, dayTimeData, nightTimeData);

        return new GetWeatherDataLastMonthResponse(
            startDateTime,
            DateOnly.FromDateTime(zoneAdjustedTime),
            dayTimeData,
            nightTimeData);
    }

    private DateOnly GetStartDateTime(
        DateTime zoneAdjustedTime,
        IEnumerable<LastMonthDailyData> dayTimeData,
        IEnumerable<LastMonthDailyData> nightTimeData)
    {
        var dayStartDateTime = dayTimeData.FirstOrDefault().Date;
        var nightStartDateTime = nightTimeData.FirstOrDefault().Date;
        var startDateTime = dayStartDateTime < nightStartDateTime
            ? dayStartDateTime
            : nightStartDateTime;

        return startDateTime == default
            ? DateOnly.FromDateTime(zoneAdjustedTime.AddDays(-30))
            : startDateTime;
    }
}
