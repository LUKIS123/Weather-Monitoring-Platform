using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DeviceManagement.Infrastructure;
using WeatherMonitor.Server.Infrastructure.Models;
using WeatherMonitor.Server.Infrastructure.Repositories;
using WeatherMonitor.Server.Infrastructure.Utility;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Repositories;

namespace WeatherMonitor.Server.Infrastructure;
public static class InfrastructureModule
{
    private const string DbConnection = "MSSQL";
    private const string TimeZoneSetting = "AppTimeZone";
    private const string Auth0ApiConfig = "Auth0Api";

    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString(DbConnection)
                                  ?? throw new ArgumentNullException(nameof(configuration), DbConnection);
        services.AddTransient<IDbConnectionFactory>(_ => new SqlDbConnectionFactory(sqlConnectionString));

        SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
        // Repositories
        services.AddTransient<IUserAuthorizationRepository, SqlUserRepository>();
        services.AddTransient<IDeviceManagementRepository, DevicesRepository>();
        services.AddTransient<IWeatherStationsRepository, DevicesRepository>();
        services.AddTransient<IDataViewRepository, DataViewRepository>();

        // Microservice Http Client
        services.AddTransient<ICoreMicroserviceHttpClientWrapper, CoreMicroserviceHttpClientWrapper>();

        var auth0ConfigSection = configuration.GetSection(Auth0ApiConfig)
                          ?? throw new ArgumentNullException(nameof(configuration), Auth0ApiConfig);
        var auth0Config = new Auth0ApiConfiguration();
        auth0ConfigSection.Bind(auth0Config);
        services.AddSingleton(auth0Config);

        services.AddTransient(_ => TimeProvider.System);

        var timeZoneId = configuration.GetValue<string>(TimeZoneSetting)
                         ?? throw new ArgumentNullException(nameof(configuration), TimeZoneSetting);
        services.AddTransient<ITimeZoneProvider>(_ => new TimeZoneProvider(timeZoneId));

        services.AddTransient<IUserAccessor, JwtUserAccessor>();

        return services;
    }
}
