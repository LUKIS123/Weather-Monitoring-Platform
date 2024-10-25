using WeatherMonitorCore.MqttDataSubscriberService.Configuration;
using WeatherMonitorCore.MqttDataSubscriberService.Utils;

namespace WeatherMonitorCore.MqttDataSubscriberService.Extensions;
public static class MqttSubscriberServiceModule
{
    private const string MqttBrokerSettings = "MqttBroker";

    public static IServiceCollection AddMqttSubscriberServiceWorkerModule(this IServiceCollection services, IConfiguration configuration)
    {
        var mqttBrokerSection = configuration.GetSection(MqttBrokerSettings)
                                ?? throw new ArgumentNullException(nameof(configuration), MqttBrokerSettings);
        var brokerConnectionSettings = new MqttBrokerConnection();
        mqttBrokerSection.Bind(brokerConnectionSettings);
        services.AddSingleton(brokerConnectionSettings);

        services.AddTransient<IServiceWorkerMqttClientGenerator, ServiceWorkerMqttClientGenerator>();
        services.AddTransient<IMqttDataService, MqttDataService>();
        services.AddHostedService<MqttSubscriptionsHandlingWorker>();

        return services;
    }
}
