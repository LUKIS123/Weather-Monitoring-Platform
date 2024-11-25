using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.DataView.Features.GetWeatherDataLastDay;

internal interface IGetWeatherDataLastDayService
{
    Task<Result<GetWeatherDataLastDayResponse>> Handle(int? deviceId, string? plusCodeSearch);
}

internal class GetWeatherDataLastDayService : IGetWeatherDataLastDayService
{
    private readonly ITimeZoneProvider _timeZoneProvider;
    private readonly TimeProvider _timeProvider;
    private readonly IDataViewRepository _dataViewRepository;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;

    public GetWeatherDataLastDayService(
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

    public async Task<Result<GetWeatherDataLastDayResponse>> Handle(int? deviceId, string? plusCodeSearch)
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

        var result = user.Role == Role.Admin
            ? await _dataViewRepository.GetLastDayWeatherDataAsync(
                new DateTime(zoneAdjustedTime.Year, zoneAdjustedTime.Month, zoneAdjustedTime.Day, zoneAdjustedTime.Hour,
                    0, 0),
                deviceId,
                plusCodeSearch)
            : await _dataViewRepository.GetLastDayWeatherDataAsync(
                new DateTime(zoneAdjustedTime.Year, zoneAdjustedTime.Month, zoneAdjustedTime.Day, zoneAdjustedTime.Hour,
                    0, 0),
                userId,
                deviceId,
                plusCodeSearch);

        return new GetWeatherDataLastDayResponse(zoneAdjustedTime, result.HourlyData);
    }
}
