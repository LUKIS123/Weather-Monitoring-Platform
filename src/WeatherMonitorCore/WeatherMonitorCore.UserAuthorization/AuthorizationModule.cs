using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.Contract.Shared;
using WeatherMonitorCore.UserAuthorization.Features;

namespace WeatherMonitorCore.UserAuthorization;

public static class AuthorizationModule
{
    private const string UserIdClaimType = "UserId";

    public static IServiceCollection AddUserAuthorizationModule(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("IsAdminPolicy", policy =>
            {
                policy.Requirements.Add(new UserIsAdminRequirement([Role.Admin], UserIdClaimType));
            });

        services.AddScoped<IAuthorizationHandler, UserIsAdminRequirementHandler>();

        return services;
    }
}
