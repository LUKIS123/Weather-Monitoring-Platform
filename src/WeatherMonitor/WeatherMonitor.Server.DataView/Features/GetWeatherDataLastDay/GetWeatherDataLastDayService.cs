using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastDay;

internal interface IGetWeatherDataLastDayService
{
    Task<Result<GetWeatherDataLastDayResponse>> Handle(int? deviceId);
}

internal class GetWeatherDataLastDayService : IGetWeatherDataLastDayService
{
    private readonly ITimeZoneProvider _timeZoneProvider;
    private readonly TimeProvider _timeProvider;
    private readonly IDataViewRepository _dataViewRepository;

    public GetWeatherDataLastDayService(
        ITimeZoneProvider timeZoneProvider,
        TimeProvider timeProvider,
        IDataViewRepository dataViewRepository)
    {
        _timeZoneProvider = timeZoneProvider;
        _timeProvider = timeProvider;
        _dataViewRepository = dataViewRepository;
    }

    public async Task<Result<GetWeatherDataLastDayResponse>> Handle(int? deviceId)
    {
        var zoneAdjustedTime =
            TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());

        var result = await _dataViewRepository.GetLastDayWeatherDataAsync(
            new DateTime(zoneAdjustedTime.Year, zoneAdjustedTime.Month, zoneAdjustedTime.Day, zoneAdjustedTime.Hour, 0, 0),
            deviceId);

        return new GetWeatherDataLastDayResponse(zoneAdjustedTime, result.HourlyData);
    }
}
