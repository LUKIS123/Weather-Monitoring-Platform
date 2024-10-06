using Microsoft.AspNetCore.Authorization;

namespace WeatherMonitorCore.UserAuthorization.Features;

internal class UserIsAdminRequirement : IAuthorizationRequirement
{
}