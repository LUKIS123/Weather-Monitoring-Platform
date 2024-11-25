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

        var stationPermissionStatusTask = _stationsPermissionsRepository.GetStationPermissionStatusAsync(stationId, userId);
        var permissionTask = _stationsPermissionsRepository.GetUsersPermissionAsync(userId, stationId);

        await Task.WhenAll(stationPermissionStatusTask, permissionTask);
        var stationPermissionStatus = await stationPermissionStatusTask;
        var permission = await permissionTask;


        if (stationPermissionStatus.UserRole == Role.Admin)
        {
            return Result.OnError(new BadRequestException("Admins already have permission to access the station"));
        }

        if (stationPermissionStatus.PermissionStatus is not (null or PermissionStatus.None))
        {
            return Result.OnError(new BadRequestException("Permission request already sent"));
        }

        if (permission is not null || !permission.Equals(default))
        {
            return Result.OnError(new BadRequestException("User has permission to station"));
        }

        var zoneAdjustedTime =
            TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());
        await _stationsPermissionsRepository.AddPermissionRequestAsync(stationId, userId, PermissionStatus.Pending, zoneAdjustedTime);

        return Result.OnSuccess();
    }
}
