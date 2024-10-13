using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitor.Server.UserAuthentication.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserAuthentication.Features.UserSettings;

internal interface IGetUserSettingsService
{
    Task<Result<UserSettingsDto>> Handle();
}

internal class GetUserSettingsService : IGetUserSettingsService
{
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _authorizationRepository;

    public GetUserSettingsService(IUserAccessor userAccessor, IUserAuthorizationRepository authorizationRepository)
    {
        _userAccessor = userAccessor;
        _authorizationRepository = authorizationRepository;
    }

    public async Task<Result<UserSettingsDto>> Handle()
    {
        if (string.IsNullOrEmpty(_userAccessor.UserId))
        {
            return new UnauthorizedException("Did not receive token");
        }

        var userAuthorizationInfo = await _authorizationRepository.GetUserAuthorizationInfoAsync(_userAccessor.UserId);

        return new UserSettingsDto
        {
            UserName = _userAccessor.UserName ?? string.Empty,
            PhotoUrl = _userAccessor.PhotoUrl,
            Email = _userAccessor.Email,
            Role = userAuthorizationInfo?.Role ?? Role.User
        };
    }
}
