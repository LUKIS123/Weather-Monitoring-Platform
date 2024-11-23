using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.GetUserPermissions;

internal interface IGetUsersPermissionsService
{
    Task<Result<PageResult<UserPermissionRequestDto>>> Handle(int pageNumber, string userId);
}

internal class GetUsersPermissionsService : IGetUsersPermissionsService
{
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;
    private readonly IUserManagementRepository _userManagementRepository;

    private const int PageSize = 10;

    public GetUsersPermissionsService(
        IUserManagementRepository userManagementRepository,
        IUserAccessor userAccessor,
        IUserAuthorizationRepository userAuthorizationRepository)
    {
        _userManagementRepository = userManagementRepository;
        _userAccessor = userAccessor;
        _userAuthorizationRepository = userAuthorizationRepository;
    }

    public async Task<Result<PageResult<UserPermissionRequestDto>>> Handle(int pageNumber, string userId)
    {
        var adminId = _userAccessor.UserId;
        if (string.IsNullOrWhiteSpace(adminId))
        {
            return new UnauthorizedException();
        }

        var admin = await _userAuthorizationRepository.GetUserAuthorizationInfoAsync(adminId);
        if (admin is null || admin.Role != Role.Admin)
        {
            return new UnauthorizedException("User must be administrator");
        }

        var user = await _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId);
        if (user is null)
        {
            return new ResourceNotFoundException("User not found");
        }

        var (userPermissions, totalItems) = await _userManagementRepository.GetUsersPermissionRequestsAsync(
            pageNumber, PageSize, userId);
        return new PageResult<UserPermissionRequestDto>(userPermissions, totalItems, PageSize, pageNumber);
    }
}
