using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.GetPendingPermissionRequests;

internal interface IGetPendingRequestsService
{
    Task<Result<PageResult<PendingPermissionDto>>> Handle(int pageNumber);
}

internal class GetPendingRequestsService : IGetPendingRequestsService
{
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;
    private readonly IUserManagementRepository _userManagementRepository;

    private const int PageSize = 10;

    public GetPendingRequestsService(
        IUserAccessor userAccessor,
        IUserAuthorizationRepository userAuthorizationRepository,
        IUserManagementRepository userManagementRepository)
    {
        _userAccessor = userAccessor;
        _userAuthorizationRepository = userAuthorizationRepository;
        _userManagementRepository = userManagementRepository;
    }

    public async Task<Result<PageResult<PendingPermissionDto>>> Handle(int pageNumber)
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

        var (pendingRequests, totalItems) = await _userManagementRepository.GetPendingPermissionRequestsAsync(
            pageNumber, PageSize, PermissionStatus.Pending);

        return new PageResult<PendingPermissionDto>(pendingRequests, totalItems, PageSize, pageNumber);
    }
}
