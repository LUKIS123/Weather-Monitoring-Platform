using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using WeatherMonitor.Server.UserAuthorization.Infrastructure;

namespace WeatherMonitor.Server.UserAuthorization.Features;
internal class UserIsAdminRequirementHandler : AuthorizationHandler<UserIsAdminRequirement>
{
    private readonly ILogger<UserIsAdminRequirement> _logger;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;

    public UserIsAdminRequirementHandler(ILogger<UserIsAdminRequirement> logger, IUserAuthorizationRepository userAuthorizationRepository)
    {
        _logger = logger;
        _userAuthorizationRepository = userAuthorizationRepository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsAdminRequirement requirement)
    {
        var userIdClaim = context.User.Claims.FirstOrDefault(x => x.Type == requirement.UserIdClaimTypeName);

        if (string.IsNullOrEmpty(userIdClaim?.Value))
        {
            context.Fail();
            return;
        }

        var authorizationInfo = await _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userIdClaim.Value);
        if (authorizationInfo is null)
        {
            context.Fail();
            return;
        }

        if (!requirement.AdminRoleTypes.Any(role => role == authorizationInfo.Role))
        {
            context.Fail();
            return;
        }

        _logger.LogInformation("User: {UserId} with admin privileges invoking action", userIdClaim);
        context.Succeed(requirement);
    }
}
