using Microsoft.AspNetCore.Authorization;

namespace WeatherMonitorCore.UserAuthorization.Features
{
    internal class UserLoggedInRequirementHandler : AuthorizationHandler<LoggedInRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LoggedInRequirement requirement)
        {
            foreach (var claimTypeName in requirement.RequiredClaimTypes)
            {
                var userClaim = context.User?.Claims?.FirstOrDefault(x => x.Type == claimTypeName);

                if (string.IsNullOrEmpty(userClaim?.Value))
                {
                    context.Fail();
                    return Task.CompletedTask;
                }
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
