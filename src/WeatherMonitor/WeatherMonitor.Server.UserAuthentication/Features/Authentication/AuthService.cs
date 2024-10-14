using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.UserAuthentication.Infrastructure.Models;

namespace WeatherMonitor.Server.UserAuthentication.Features.Authentication;
internal interface IAuthService
{
    UserInfo Handle();
}

internal class AuthService : IAuthService
{
    private readonly IUserAccessor _userAccessor;

    public AuthService(IUserAccessor userAccessor)
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
            Role = _userAccessor.Role
        };
    }
}
