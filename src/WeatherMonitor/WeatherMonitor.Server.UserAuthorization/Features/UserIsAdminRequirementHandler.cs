using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserAuthorization.Features;
internal class UserIsAdminRequirementHandler : AuthorizationHandler<UserIsAdminRequirement>
{
    private readonly ILogger<UserIsAdminRequirement> _logger;

    public UserIsAdminRequirementHandler(ILogger<UserIsAdminRequirement> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIsAdminRequirement requirement)
    {
        var userIdClaim = context.User.Claims.FirstOrDefault(x => x.Type == requirement.UserIdClaimTypeName);
        if (string.IsNullOrEmpty(userIdClaim?.Value))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var roleClaim = context.User.Claims.FirstOrDefault(x => x.Type == requirement.RoleClaimTypeName);
        if (string.IsNullOrEmpty(roleClaim?.Value))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (!int.TryParse(roleClaim.Value, out var roleTypeValue))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (!Enum.IsDefined(typeof(Role), roleTypeValue))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var roleEnumResult = (Role)roleTypeValue;
        if (!requirement.AdminRoleTypes.Any(role => role == roleEnumResult))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        _logger.LogInformation("User: {UserId} with admin privileges invoking action", userIdClaim);
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
