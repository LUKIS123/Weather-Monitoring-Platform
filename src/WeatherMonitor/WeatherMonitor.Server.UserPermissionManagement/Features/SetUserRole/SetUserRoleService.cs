using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitorCore.Contract.Shared;
using WeatherMonitorCore.Contract.UserAuthenticationModule;

namespace WeatherMonitor.Server.UserPermissionManagement.Features.SetUserRole;

internal interface ISetUserRoleService
{
    Task<Result> Handle(SetUserRoleRequest setUserRoleRequest);
}

internal class SetUserRoleService : ISetUserRoleService
{
    private readonly ICoreMicroserviceHttpClientWrapper _coreMicroserviceHttpClientWrapper;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;

    public SetUserRoleService(ICoreMicroserviceHttpClientWrapper coreMicroserviceHttpClientWrapper, IUserAccessor userAccessor, IUserAuthorizationRepository userAuthorizationRepository)
    {
        _coreMicroserviceHttpClientWrapper = coreMicroserviceHttpClientWrapper;
        _userAccessor = userAccessor;
        _userAuthorizationRepository = userAuthorizationRepository;
    }

    public async Task<Result> Handle(SetUserRoleRequest setUserRoleRequest)
    {
        var adminUserId = _userAccessor.UserId;
        if (string.IsNullOrWhiteSpace(adminUserId))
        {
            return Result.OnError(new UnauthorizedException());
        }

        var admin = await _userAuthorizationRepository.GetUserAuthorizationInfoAsync(adminUserId);
        if (admin is null || admin.Role != Role.Admin)
        {
            return Result.OnError(new UnauthorizedException("User must be administrator"));
        }

        var (success, message) = await _coreMicroserviceHttpClientWrapper.PostHttpRequest<UpdateRoleRequest>(
           "api/user/setRole",
           new UpdateRoleRequest
           {
               Role = setUserRoleRequest.Role,
               UserId = setUserRoleRequest.UserId
           },
           _userAccessor.Token);

        return !success
            ? Result.OnError(new MicroserviceApiException(message))
            : Result.OnSuccess();
    }
}
