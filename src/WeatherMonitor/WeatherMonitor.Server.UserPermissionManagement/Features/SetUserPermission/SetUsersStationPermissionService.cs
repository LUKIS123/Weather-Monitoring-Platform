using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.SetUserPermission;

internal class SetUsersStationPermissionService
{
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;
    private readonly IUserManagementRepository _userManagementRepository;

    public SetUsersStationPermissionService(
        IUserAccessor userAccessor,
        IUserAuthorizationRepository userAuthorizationRepository,
        IUserManagementRepository userManagementRepository)
    {
        _userAccessor = userAccessor;
        _userAuthorizationRepository = userAuthorizationRepository;
        _userManagementRepository = userManagementRepository;
    }

    // 1 denied - zmien status na denied
    // 2 zmien na approved + dodanie do tabeli StationPermissions, jesli istnieje permission to blad
    // 3 zmien na denied + usuniecie z tabeli StationPermissions
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
            userId, updatePermissionRequest.DeviceId);
        var userPermissionTask = _userManagementRepository.GetUserPermissionAsync(
            userId, updatePermissionRequest.DeviceId);
        await Task.WhenAll(permissionRequestTask, userPermissionTask);

        var permissionRequest = await permissionRequestTask;
        var userPermission = await userPermissionTask;

        if (permissionRequest is null)
        {
            return new UnauthorizedException("Permission request does not exist");
        }

        if (permissionRequest.Value.PermissionStatus == PermissionStatus.Granted
            && updatePermissionRequest.Status == PermissionStatus.Granted)
        {
            return new BadRequestException("User already has permission to the station");
        }

        if (permissionRequest.Value.PermissionStatus == PermissionStatus.Pending
            && updatePermissionRequest.Status == PermissionStatus.Denied)
        {
            var result = await _userManagementRepository.SetPermissionRequestAsync(
                userId,
                updatePermissionRequest.UserId,
                updatePermissionRequest.Status);
            return new UpdatePermissionResponse(result, null);
        }

        if (permissionRequest.Value.PermissionStatus == PermissionStatus.Pending
            && updatePermissionRequest.Status == PermissionStatus.Granted)
        {
            if (userPermission is not null)
            {
                return new BadRequestException("User already has permission to the station");
            }

            var result = await _userManagementRepository.AddUserStationPermissionAsync(
                userId,
                updatePermissionRequest.DeviceId,
                PermissionStatus.Granted);

            return new UpdatePermissionResponse(result.request, result.permission);
        }

        if (permissionRequest.Value.PermissionStatus == PermissionStatus.Granted
            && updatePermissionRequest.Status == PermissionStatus.Denied)
        {
            if (userPermission is null)
            {
                return new BadRequestException("User does not have permission to the station");
            }

            var result = await _userManagementRepository.RemoveUserStationPermissionAsync(
                userId,
                updatePermissionRequest.DeviceId,
                PermissionStatus.Denied);

            return new UpdatePermissionResponse(result, null);
        }

        return new BadRequestException();
    }
}
