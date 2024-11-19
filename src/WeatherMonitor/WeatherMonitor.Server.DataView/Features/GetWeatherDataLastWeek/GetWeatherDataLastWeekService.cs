using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastWeek;

internal interface IGetWeatherDataLastWeekService
{
    Task<Result<GetWeatherDataLastWeekResponse>> Handle(int? deviceId, string? plusCodeSearch);
}

internal class GetWeatherDataLastWeekService : IGetWeatherDataLastWeekService
{
    private readonly ITimeZoneProvider _timeZoneProvider;
    private readonly TimeProvider _timeProvider;
    private readonly IDataViewRepository _dataViewRepository;

    public GetWeatherDataLastWeekService(
        ITimeZoneProvider timeZoneProvider,
        TimeProvider timeProvider,
        IDataViewRepository dataViewRepository)
    {
        _timeZoneProvider = timeZoneProvider;
        _timeProvider = timeProvider;
        _dataViewRepository = dataViewRepository;
    }

    public async Task<Result<GetWeatherDataLastWeekResponse>> Handle(int? deviceId, string? plusCodeSearch)
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

        var result = await _dataViewRepository.GetLastWeekWeatherDataAsync(
            zoneAdjustedTime,
            deviceId,
            plusCodeSearch);

        var startDateTime = result.LastWeekHourlyData.FirstOrDefault().HourDateTime;
        startDateTime = startDateTime == default
            ? zoneAdjustedTime.AddDays(-7)
            : startDateTime;

        return new GetWeatherDataLastWeekResponse(startDateTime, zoneAdjustedTime, result.LastWeekHourlyData);
    }
}
