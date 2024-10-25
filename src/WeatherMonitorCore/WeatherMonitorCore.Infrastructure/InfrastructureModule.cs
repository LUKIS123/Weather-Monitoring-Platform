using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.Infrastructure.MqttEventHandlers;
using WeatherMonitorCore.Infrastructure.Repositories;
using WeatherMonitorCore.Infrastructure.Utility;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.MqttAuth.Infrastructure;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;
using WeatherMonitorCore.SharedKernel.Infrastructure;
using WeatherMonitorCore.SharedKernel.Infrastructure.Repositories;
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
        services.AddTransient<IUserAuthorizationRepository, SqlUserRepository>();
        services.AddTransient<IDeviceManagementRepository, DevicesRepository>();
        services.AddTransient<IMqttClientAuthenticationRepository, MqttCredentialsRepository>();
        services.AddTransient<IAppMqttClientsRepository, MqttCredentialsRepository>();
        services.AddTransient<ISensorDataRepository, SensorDataRepository>();

        services.AddTransient<IDeviceMqttMessageParser, DeviceMqttMessageParser>();
        services.AddTransient(_ => TimeProvider.System);
        if (infrastructureType == InfrastructureType.AspNetCore)
        {
            services.AddTransient<IUserAccessor, JwtUserAccessor>();

            // Add Mqtt Event Handlers
            services.AddTransient<IMqttEventHandler, WeatherDataSavingHandler>();
        }

        return services;
    }
}