using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.SharedKernel.Infrastructure;
using WeatherMonitorCore.SharedKernel.Models;
using WeatherMonitorCore.UserAuthorization.Features;

namespace WeatherMonitorCore.UserAuthorization;

public static class UserAuthorizationModule
{
    private const string UserIdClaimType = "UserId";
    private const string UserNameClaimType = "UserName";

    public static IServiceCollection AddUserAuthorizationModule(this IServiceCollection services, InfrastructureType infrastructureType = InfrastructureType.AspNetCore)
    {
        if (infrastructureType == InfrastructureType.AspNetCore)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy("UserLoggedInPolicy", policy =>
                {
                    policy.Requirements.Add(new LoggedInRequirement([UserIdClaimType, UserNameClaimType]));
                })
                .AddPolicy("IsAdminPolicy", policy =>
                {
                    policy.Requirements.Add(new UserIsAdminRequirement([Role.Admin], UserIdClaimType));
                });

            services.AddScoped<IAuthorizationHandler, UserLoggedInRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, UserIsAdminRequirementHandler>();
        }

        return services;
    }
}