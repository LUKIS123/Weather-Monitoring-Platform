using WeatherMonitorCore.MqttDataSubscriberService.Utils;

namespace WeatherMonitorCore.MqttDataSubscriberService.Extensions;
public static class MqttSubscriberServiceModule
{
    public static IServiceCollection AddMqttSubscriberServiceWorkerModule(this IServiceCollection services)
    {
        services.AddTransient<IServiceWorkerMqttClientGenerator, ServiceWorkerMqttClientGenerator>();
        services.AddTransient<IMqttDataService, MqttDataService>();
        services.AddHostedService<MqttSubscriptionsHandlingWorker>();

        return services;
    }
}
