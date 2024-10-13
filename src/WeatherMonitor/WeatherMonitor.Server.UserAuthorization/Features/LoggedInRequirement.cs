using Microsoft.AspNetCore.Authorization;

namespace WeatherMonitor.Server.UserAuthorization.Features;
internal class LoggedInRequirement : IAuthorizationRequirement
{
    public IEnumerable<string> RequiredClaimTypes { get; }

    public LoggedInRequirement(IEnumerable<string> requiredClaimTypes)
    {
        RequiredClaimTypes = requiredClaimTypes;
    }
}
