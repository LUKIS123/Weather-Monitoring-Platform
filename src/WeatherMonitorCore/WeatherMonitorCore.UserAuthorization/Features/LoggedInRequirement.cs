using Microsoft.AspNetCore.Authorization;

namespace WeatherMonitorCore.UserAuthorization.Features;

internal class LoggedInRequirement : IAuthorizationRequirement
{
    public List<string> RequiredClaimTypes { get; }

    public LoggedInRequirement(List<string> requiredClaimTypes)
    {
        RequiredClaimTypes = requiredClaimTypes;
    }
}
