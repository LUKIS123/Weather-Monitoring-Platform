using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.StationsPermissions.Features.StationPermissionStatus;

internal interface IGetStationPermissionStatusService
{
    Task<Result<StationPermissionStatusResponse>> Handle(int stationId);
}

internal class GetStationPermissionStatusService : IGetStationPermissionStatusService
{
    private readonly IUserAccessor _userAccessor;
    private readonly IStationsPermissionsRepository _stationPermissionRepository;

    public GetStationPermissionStatusService(IUserAccessor userAccessor, IStationsPermissionsRepository stationPermissionRepository)
    {
        _userAccessor = userAccessor;
        _stationPermissionRepository = stationPermissionRepository;
    }

    public async Task<Result<StationPermissionStatusResponse>> Handle(int stationId)
    {
        var userId = _userAccessor.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new UnauthorizedException();
        }

        var stationPermissionDto = await _stationPermissionRepository.GetStationPermissionStatusAsync(stationId, userId);

        var hasPermission = HasPermission(stationPermissionDto);
        var canRequestPermission = CanSendPermissionRequest(stationPermissionDto);

        return new StationPermissionStatusResponse(stationPermissionDto, hasPermission, canRequestPermission);
    }

    private static bool HasPermission(StationUserPermissionDto permissionDto)
    {
        if (permissionDto.UserRole == Role.Admin)
        {
            return true;
        }

        if (permissionDto.PermissionStatus is not null
            && permissionDto.UserRole != Role.Admin)
        {
            return permissionDto is
            {
                PermissionStatus: PermissionStatus.Granted,
                PermissionRecordExists: true
            };
        }

        return false;
    }

    private static bool CanSendPermissionRequest(StationUserPermissionDto permissionDto) =>
        (permissionDto.PermissionStatus is null or PermissionStatus.NotRequested || !permissionDto.PermissionRecordExists)
        && permissionDto.UserRole != Role.Admin;
}
