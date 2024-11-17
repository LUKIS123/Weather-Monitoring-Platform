using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

namespace WeatherMonitor.Server.StationsPermissions.Features.GetUsersPermissions;

internal interface IGetUsersPermissionsService
{
    Task<Result<PageResult<UsersPermissionRequestDto>>> Handle(int pageNumber);
}

internal class GetUsersPermissionsService : IGetUsersPermissionsService
{
    private readonly IStationsPermissionsRepository _stationsPermissionsRepository;
    private readonly IUserAccessor _userAccessor;

    private const int PageSize = 10;

    public GetUsersPermissionsService(IStationsPermissionsRepository stationsPermissionsRepository, IUserAccessor userAccessor)
    {
        _stationsPermissionsRepository = stationsPermissionsRepository;
        _userAccessor = userAccessor;
    }

    public async Task<Result<PageResult<UsersPermissionRequestDto>>> Handle(int pageNumber)
    {
        var userId = _userAccessor.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new UnauthorizedException();
        }

        var (stations, totalItems) = await _stationsPermissionsRepository.GetPermissionRequestsAsync(pageNumber, PageSize, userId);

        return new PageResult<UsersPermissionRequestDto>(stations, totalItems, PageSize, pageNumber);
    }
}
