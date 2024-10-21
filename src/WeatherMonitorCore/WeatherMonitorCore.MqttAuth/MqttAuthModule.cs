using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.MqttAuth.Features.AclCheck;
using WeatherMonitorCore.MqttAuth.Features.BrokerClientAuthentication;
using WeatherMonitorCore.MqttAuth.Features.SuperUserCheck;

namespace WeatherMonitorCore.MqttAuth;
public static class MqttAuthModule
{
    public static IServiceCollection AddMqttBrokerAuthModule(this IServiceCollection services)
    {
        services.AddTransient<IAclCheckService, AclCheckService>();
        services.AddTransient<ISuperUserCheckService, SuperUserCheckService>();
        services.AddTransient<IBrokerClientAuthenticationService, BrokerClientAuthenticationService>();

        return services;
    }
}
