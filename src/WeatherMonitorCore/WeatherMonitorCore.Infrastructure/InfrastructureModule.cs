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
    private const string TimeZoneSetting = "AppTimeZone";
    private const string EncryptionSettings = "EncryptionSettings";

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

        var timeZoneId = configuration.GetValue<string>(TimeZoneSetting)
                         ?? throw new ArgumentNullException(nameof(configuration), TimeZoneSetting);
        services.AddTransient<ITimeZoneProvider>(_ => new TimeZoneProvider(timeZoneId));

        var encryptionKeys = configuration.GetSection(EncryptionSettings)
                             ?? throw new ArgumentNullException(nameof(configuration), EncryptionSettings);
        var encryptionSettings = new EncryptionSettings();
        encryptionKeys.Bind(encryptionSettings);
        services.AddSingleton(encryptionSettings);
        services.AddTransient<IAesEncryptionHelper, AesEncryptionHelper>();

        if (infrastructureType == InfrastructureType.AspNetCore)
        {
            services.AddTransient<IUserAccessor, JwtUserAccessor>();

            // Add Mqtt Event Handlers
            services.AddTransient<IMqttEventHandler, WeatherDataSavingHandler>();
        }

        return services;
    }
}