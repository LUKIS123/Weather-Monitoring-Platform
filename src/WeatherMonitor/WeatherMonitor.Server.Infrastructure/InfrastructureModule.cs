using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.Interfaces;

namespace WeatherMonitor.Server.Infrastructure;
public static class InfrastructureModule
{
    private const string DbConnection = "MS-SQL";

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString(DbConnection)
                                  ?? throw new ArgumentNullException(nameof(configuration), DbConnection);
        services.AddTransient<IDbConnectionFactory>(_ => new SqlDbConnectionFactory(sqlConnectionString));

        services.AddTransient<IUserAuthorizationRepository, SqlUserRepository>();

        services.AddTransient(_ => TimeProvider.System);

        services.AddTransient<IUserAccessor, JwtUserAccessor>();

        return services;
    }
}
