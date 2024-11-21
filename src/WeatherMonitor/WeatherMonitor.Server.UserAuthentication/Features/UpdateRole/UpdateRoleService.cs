using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitorCore.Contract.Shared;
using WeatherMonitorCore.Contract.UserAuthenticationModule;

namespace WeatherMonitor.Server.UserAuthentication.Features.UpdateRole;

internal interface IUpdateRoleService
{
    Task<Result> Handle(string userId, Role role);
}

internal class UpdateRoleService : IUpdateRoleService
{
    private readonly ICoreMicroserviceHttpClientWrapper _httpClientWrapper;

    public UpdateRoleService(ICoreMicroserviceHttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public async Task<Result> Handle(string userId, Role role)
    {
        var (success, message) = await _httpClientWrapper.PostHttpRequest(
            "/api/user/setRole",
            new UpdateRoleRequest { UserId = userId, Role = role });

        if (!success)
        {
            return Result.OnError(new MicroserviceApiException(message));
        }

        return Result.OnSuccess();
    }
}
