using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.Infrastructure.Repositories;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.SharedKernel.Infrastructure;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Repositories;

namespace WeatherMonitorCore.Infrastructure;

public static class InfrastructureModule
{
    private const string DbConnection = "MS-SQL";

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration, InfrastructureType infrastructureType = InfrastructureType.AspNetCore)
    {
        var sqlConnectionString = configuration.GetConnectionString(DbConnection)
                                  ?? throw new ArgumentNullException(nameof(configuration), DbConnection);
        services.AddTransient<IDbConnectionFactory>(_ => new SqlDbConnectionFactory(sqlConnectionString));

        services.AddTransient<IUserSettingsRepository, SqlUserRepository>();

        services.AddTransient(_ => TimeProvider.System);

        if (infrastructureType == InfrastructureType.AspNetCore)
        {
            services.AddTransient<IUserAccessor, JwtUserAccessor>();
        }

        return services;
    }
}