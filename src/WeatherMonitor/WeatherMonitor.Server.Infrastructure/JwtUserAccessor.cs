using Microsoft.AspNetCore.Http;
using WeatherMonitor.Server.Interfaces;

namespace WeatherMonitor.Server.Infrastructure;
internal class JwtUserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string UserIdClaim = "UserId";
    private const string UserNameClaim = "UserName";
    private const string PhotoUrlClaim = "PhotoUrl";

    public JwtUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            var claim = claims?.FirstOrDefault(x => x.Type == UserIdClaim);

            return claim?.Value;
        }
    }

    public string? UserName
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            var claim = claims?.FirstOrDefault(x => x.Type == UserNameClaim);

            return claim?.Value;
        }
    }

    public string? PhotoUrl
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            var claim = claims?.FirstOrDefault(x => x.Type == PhotoUrlClaim);

            return claim?.Value;
        }
    }
}
