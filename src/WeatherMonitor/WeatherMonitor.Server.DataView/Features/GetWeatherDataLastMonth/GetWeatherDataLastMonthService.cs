using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastMonth;

internal interface IGetWeatherDataLastMonthService
{
    Task<Result<GetWeatherDataLastMonthResponse>> Handle(int? deviceId, string? plusCodeSearch);
}

internal class GetWeatherDataLastMonthService : IGetWeatherDataLastMonthService
{
    private readonly ITimeZoneProvider _timeZoneProvider;
    private readonly TimeProvider _timeProvider;
    private readonly IDataViewRepository _dataViewRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;

    public GetWeatherDataLastMonthService(
        ITimeZoneProvider timeZoneProvider,
        TimeProvider timeProvider,
        IDataViewRepository dataViewRepository,
        IUserAccessor userAccessor,
        IUserAuthorizationRepository userAuthorizationRepository)
    {
        _timeZoneProvider = timeZoneProvider;
        _timeProvider = timeProvider;
        _dataViewRepository = dataViewRepository;
        _userAccessor = userAccessor;
        _userAuthorizationRepository = userAuthorizationRepository;
    }

    public async Task<Result<GetWeatherDataLastMonthResponse>> Handle(int? deviceId, string? plusCodeSearch)
    {
        var userId = _userAccessor.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new UnauthorizedException("User not authenticated");
        }

        var user = await _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId);
        if (user is null)
        {
            return new UnauthorizedException("User not found");
        }

        var zoneAdjustedTime =
            TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());
        zoneAdjustedTime = new DateTime(
            zoneAdjustedTime.Year,
            zoneAdjustedTime.Month,
            zoneAdjustedTime.Day,
            zoneAdjustedTime.Hour,
            0,
            0);

        var dayTimeResultsTask = user.Role == Role.Admin
            ? _dataViewRepository.GetDayTimeLastMonthWeatherDataAsync(
                zoneAdjustedTime,
                deviceId,
                plusCodeSearch)
            : _dataViewRepository.GetDayTimeLastMonthWeatherDataAsync(
                zoneAdjustedTime,
                userId,
                deviceId,
                plusCodeSearch);

        var nightTimeResultsTask = user.Role == Role.Admin
            ? _dataViewRepository.GetNightTimeLastMonthWeatherDataAsync(
                zoneAdjustedTime,
                deviceId,
                plusCodeSearch)
            : _dataViewRepository.GetNightTimeLastMonthWeatherDataAsync(
                zoneAdjustedTime,
                userId,
                deviceId,
                plusCodeSearch);

        await Task.WhenAll(dayTimeResultsTask, nightTimeResultsTask);

        var dayTimeData = (await dayTimeResultsTask).DailyData.ToArray();
        var nightTimeData = (await nightTimeResultsTask).DailyData.ToArray();

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
