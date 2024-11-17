using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.Shared.MqttClient.Features.MqttFactoryHelpers;
using WeatherMonitorCore.Shared.MqttClient.Features.SubscribedTopicsCache;
using WeatherMonitorCore.Shared.MqttClient.Features.Subscriptions;
using WeatherMonitorCore.Shared.MqttClient.Interfaces;

namespace WeatherMonitorCore.Shared.MqttClient;

public static class MqttClientModule
{
    public static IServiceCollection AddMqttClientModule(this IServiceCollection services)
    {
        services.AddSingleton<IMqttFactoryWrapper, MqttFactoryWrapper>();
        services.AddSingleton<ITopicsCache, TopicsCache>();
        services.AddSingleton<ISubscriptionsManagingService, SubscriptionsManagingService>();

        return services;
    }
}
