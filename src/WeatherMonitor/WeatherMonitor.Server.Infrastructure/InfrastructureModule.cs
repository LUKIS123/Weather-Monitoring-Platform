using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.DeviceManagement.Infrastructure;
using WeatherMonitor.Server.Infrastructure.Repositories;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Repositories;

namespace WeatherMonitor.Server.Infrastructure;
public static class InfrastructureModule
{
    private const string DbConnection = "MS-SQL";

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString(DbConnection)
                                  ?? throw new ArgumentNullException(nameof(configuration), DbConnection);
        services.AddTransient<IDbConnectionFactory>(_ => new SqlDbConnectionFactory(sqlConnectionString));

        // Repositories
        services.AddTransient<IUserAuthorizationRepository, SqlUserRepository>();
        services.AddTransient<IDeviceManagementRepository, DevicesRepository>();

        // Microservice Http Client
        services.AddTransient<ICoreMicroserviceHttpClientWrapper, CoreMicroserviceHttpClientWrapper>();

        services.AddTransient(_ => TimeProvider.System);
        services.AddTransient<IUserAccessor, JwtUserAccessor>();

        return services;
    }
}
