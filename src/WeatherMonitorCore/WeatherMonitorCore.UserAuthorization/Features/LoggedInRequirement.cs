using Microsoft.AspNetCore.Authorization;

namespace WeatherMonitorCore.UserAuthorization.Features;

internal class LoggedInRequirement : IAuthorizationRequirement
{
    public IEnumerable<string> RequiredClaimTypes { get; }

    public LoggedInRequirement(IEnumerable<string> requiredClaimTypes)
    {
        RequiredClaimTypes = requiredClaimTypes;
    }
}
