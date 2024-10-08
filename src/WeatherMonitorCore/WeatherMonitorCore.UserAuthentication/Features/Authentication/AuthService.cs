using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Models;

namespace WeatherMonitorCore.UserAuthentication.Features.Authentication;

internal interface IAuthenticationService
{
    UserInfo Handle();
}

internal class AuthenticationService : IAuthenticationService
{
    private readonly IUserAccessor _userAccessor;

    public AuthenticationService(IUserAccessor userAccessor)
    {
        _userAccessor = userAccessor;
    }

    public UserInfo Handle()
    {
        return new UserInfo()
        {
            UserId = _userAccessor.UserId ?? string.Empty,
            UserName = _userAccessor.UserName ?? string.Empty,
            PhotoUrl = _userAccessor.PhotoUrl ?? string.Empty,
            Email = _userAccessor.Email ?? string.Empty,
        };
    }
}