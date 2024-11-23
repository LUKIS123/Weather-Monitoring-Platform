using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.GetUsers;

internal interface IGetUsersService
{
    Task<Result<PageResult<UserDto>>> Handle(int pageNumber, string? nicknameSearch = null);
}

internal class GetUsersService : IGetUsersService
{
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;
    private readonly IUserManagementRepository _userManagementRepository;

    private const int PageSize = 10;

    public GetUsersService(
        IUserManagementRepository userManagementRepository,
        IUserAccessor userAccessor,
        IUserAuthorizationRepository userAuthorizationRepository)
    {
        _userManagementRepository = userManagementRepository;
        _userAccessor = userAccessor;
        _userAuthorizationRepository = userAuthorizationRepository;
    }

    public async Task<Result<PageResult<UserDto>>> Handle(int pageNumber, string? nicknameSearch = null)
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

        var (users, totalItems) = await _userManagementRepository.GetUsersAsync(
            pageNumber, PageSize, nicknameSearch);
        return new PageResult<UserDto>(users, totalItems, PageSize, pageNumber);
    }
}
