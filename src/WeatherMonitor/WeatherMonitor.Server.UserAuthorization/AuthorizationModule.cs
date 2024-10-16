using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.UserAuthorization.Features;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserAuthorization;

public static class AuthorizationModule
{
    private const string UserIdClaimType = "UserId";
    private const string UserNameClaimType = "UserName";

    public static IServiceCollection AddUserAuthorizationModule(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("UserLoggedInPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new LoggedInRequirement([UserIdClaimType, UserNameClaimType]));
            })
            .AddPolicy("IsAdminPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddRequirements(new UserIsAdminRequirement([Role.Admin], UserIdClaimType));
            });

        services.AddScoped<IAuthorizationHandler, UserLoggedInRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, UserIsAdminRequirementHandler>();

        return services;
    }
}
