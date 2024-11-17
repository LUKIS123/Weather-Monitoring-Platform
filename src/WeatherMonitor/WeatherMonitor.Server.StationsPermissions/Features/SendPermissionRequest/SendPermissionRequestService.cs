using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.StationsPermissions.Features.SendPermissionRequest;

internal interface ISendPermissionRequestService
{
    Task<Result> Handle(int stationId);
}

internal class SendPermissionRequestService : ISendPermissionRequestService
{
    private readonly IUserAccessor _userAccessor;
    private readonly IStationsPermissionsRepository _stationsPermissionsRepository;
    private readonly ITimeZoneProvider _timeZoneProvider;
    private readonly TimeProvider _timeProvider;

    public SendPermissionRequestService(
        IUserAccessor userAccessor,
        IStationsPermissionsRepository stationsPermissionsRepository,
        ITimeZoneProvider timeZoneProvider,
        TimeProvider timeProvider)
    {
        _userAccessor = userAccessor;
        _stationsPermissionsRepository = stationsPermissionsRepository;
        _timeZoneProvider = timeZoneProvider;
        _timeProvider = timeProvider;
    }

    public async Task<Result> Handle(int stationId)
    {
        var userId = _userAccessor.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Result.OnError(new UnauthorizedException());
        }

        var stationPermissionDto = await _stationsPermissionsRepository.GetStationPermissionStatusAsync(stationId, userId);
        if (stationPermissionDto.UserRole == Role.Admin)
        {
            return Result.OnError(new BadRequestException("Admins already have permission to access the station"));
        }

        if (stationPermissionDto.PermissionStatus is not null)
        {
            return Result.OnError(new BadRequestException("Permission request already sent"));
        }

        var zoneAdjustedTime =
            TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());
        await _stationsPermissionsRepository.SendPermissionRequestAsync(stationId, userId, PermissionStatus.Pending, zoneAdjustedTime);

        return Result.OnSuccess();
    }
}
