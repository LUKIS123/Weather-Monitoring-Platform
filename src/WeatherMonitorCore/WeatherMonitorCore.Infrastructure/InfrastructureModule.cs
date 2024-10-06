using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.SharedKernel.Infrastructure;

namespace WeatherMonitorCore.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string sqlConnectionString, InfrastructureType infrastructureType = InfrastructureType.AspNetCore)
    {
        services.AddTransient<IDbConnectionFactory>(_ => new SqlDbConnectionFactory(sqlConnectionString));
        // services.AddTransient<IUserSettingsRepository, SqlUserSettingsRepository>();
        // services.AddTransient<IUserTokensRepository, SqlUserSettingsRepository>();
        // services.AddTransient<ITokenRepository, SqlUserSettingsRepository>();

        services.AddTransient(_ => TimeProvider.System);

        if (infrastructureType == InfrastructureType.AspNetCore)
        {
            services.AddTransient<IUserAccessor, JwtUserAccessor>();
        }

        return services;
    }
}