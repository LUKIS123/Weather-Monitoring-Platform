using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.SetUserPermission;

internal interface ISetUsersStationPermissionService
{
    Task<Result<UpdatePermissionResponse>> Handle(UpdatePermissionRequest updatePermissionRequest);
}

internal class SetUsersStationPermissionService : ISetUsersStationPermissionService
{
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;
    private readonly IUserManagementRepository _userManagementRepository;
    private readonly ITimeZoneProvider _timeZoneProvider;
    private readonly TimeProvider _timeProvider;

    public SetUsersStationPermissionService(
        IUserAccessor userAccessor,
        IUserAuthorizationRepository userAuthorizationRepository,
        IUserManagementRepository userManagementRepository,
        ITimeZoneProvider timeZoneProvider,
        TimeProvider timeProvider)
    {
        _userAccessor = userAccessor;
        _userAuthorizationRepository = userAuthorizationRepository;
        _userManagementRepository = userManagementRepository;
        _timeZoneProvider = timeZoneProvider;
        _timeProvider = timeProvider;
    }

    public async Task<Result<UpdatePermissionResponse>> Handle(UpdatePermissionRequest updatePermissionRequest)
    {
        var userId = _userAccessor.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new UnauthorizedException();
        }

        var user = await _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId);
        if (user is null || user.Role != Role.Admin)
        {
            return new UnauthorizedException("User must be administrator");
        }

        var permissionRequestTask = _userManagementRepository.GetPermissionRequestAsync(
            updatePermissionRequest.UserId, updatePermissionRequest.DeviceId);
        var userPermissionTask = _userManagementRepository.GetUserPermissionAsync(
            updatePermissionRequest.UserId, updatePermissionRequest.DeviceId);

        await Task.WhenAll(permissionRequestTask, userPermissionTask);
        var permissionRequest = await permissionRequestTask;
        var userPermission = await userPermissionTask;

        if (permissionRequest is null)
        {
            return new UnauthorizedException("Permission request does not exist");
        }

        var zoneAdjustedTime =
            TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());

        switch (permissionRequest.Value.PermissionStatus)
        {
            case PermissionStatus.Granted
                when updatePermissionRequest.Status == PermissionStatus.Granted:
                return new BadRequestException("User already has permission to the station");

            case PermissionStatus.Pending
                when updatePermissionRequest.Status == PermissionStatus.Denied:
                {
                    var result = await _userManagementRepository.SetPermissionRequestAsync(
                        updatePermissionRequest.UserId,
                        updatePermissionRequest.DeviceId,
                        PermissionStatus.Denied,
                        zoneAdjustedTime);
                    return new UpdatePermissionResponse(result, null);
                }

            case PermissionStatus.Pending
                when updatePermissionRequest.Status == PermissionStatus.Granted:
                {
                    if (userPermission is not null && userPermission != default(UserPermissionDto))
                    {
                        return new BadRequestException("User already has permission to the station");
                    }

                    var result = await _userManagementRepository.AddUserStationPermissionAsync(
                        updatePermissionRequest.UserId,
                        updatePermissionRequest.DeviceId,
                        PermissionStatus.Granted,
                        zoneAdjustedTime);

                    return new UpdatePermissionResponse(result.request, result.permission);
                }

            case PermissionStatus.Granted
                when updatePermissionRequest.Status == PermissionStatus.Denied:
                {
                    if (userPermission is null || userPermission == default(UserPermissionDto))
                    {
                        return new BadRequestException("User does not have permission to the station");
                    }

                    var result = await _userManagementRepository.RemoveUserStationPermissionAsync(
                        updatePermissionRequest.UserId,
                        updatePermissionRequest.DeviceId,
                        PermissionStatus.Denied,
                        zoneAdjustedTime);

                    return new UpdatePermissionResponse(result, null);
                }

            default:
                return new BadRequestException();
        }
    }
}
