using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.SharedKernel.Infrastructure;
using WeatherMonitorCore.UserAuthorization.Features;

namespace WeatherMonitorCore.UserAuthorization;

public static class UserAuthorizationModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, InfrastructureType infrastructureType = InfrastructureType.AspNetCore)
    {
        if (infrastructureType == InfrastructureType.AspNetCore)
        {
            services.AddAuthorizationBuilder()
                .AddPolicy("UserLoggedInPolicy", policy =>
                {
                    policy.Requirements.Add(new LoggedInRequirement(["UserId", "UserName"]));
                });

            services.AddScoped<IAuthorizationHandler, UserLoggedInRequirementHandler>();
        }

        return services;
    }
}