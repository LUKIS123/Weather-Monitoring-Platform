using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastWeek;
internal class GetWeatherDataLastWeekService
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


    // todo: zwraca min i max w response, poza samymi danymi, zpytania zapisane w azure data studio tylko pamietaz zeby brac ostateczna wersje
    //https://chatgpt.com/c/672f997a-d650-800f-b891-440670f26080
    public async Task<LastWeekWeatherData> Handle(int? deviceId)
    {
        var zoneAdjustedTime =
            TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());

        var result = await _dataViewRepository.GetLastWeekWeatherDataAsync(zoneAdjustedTime, deviceId);

        return new GetWeatherDataLastWeekResponse(null, zoneAdjustedTime, result.LastWeekHourlyData);
    }
}
