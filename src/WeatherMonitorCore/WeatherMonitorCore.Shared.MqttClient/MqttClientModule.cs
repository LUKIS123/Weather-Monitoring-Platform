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

// PS C:\Users\lukas> mosquitto_pub -h localhost -p 1883 -t "weather/F28B0755-587D-4F9D-8FF6-ECC7277C05F2" -m "Hello MQTT3" -u "odbieracz" -P "5BFJWzxWnn0WP8xZ" -i "ODBIERACZ-A916C8DD-6A31-43EC-B9ED-D6F48A0C46FC"

// PS C:\Users\lukas> mosquitto_pub -h localhost -p 1883 -t "weather/EDA796F1-9ECD-4874-BF7E-AA1CE8392FFD" -m "Hello MQTT3" -u "Benedyktynska-1" -P "js8Yu*\KTUUsc41\" -i "BENEDYKTYNSKA-1-BD3E875F-BC53-42A4-A257-6B23A3C68944"

// PS C:\Users\lukas> mosquitto_pub -h localhost -p 1883 -t "weather/EDA796F1-9ECD-4874-BF7E-AA1CE8392FFD" -m "{`"MeasuredAt`": `"2024-10-25T14:30:00Z`",`"Humidity`": 55.3,`"Temperature`": 22.1,`"AirPressure`": 1013.25,`"PM1_0`": 12.4,`"PM2_5`": 18.9,`"PM10`": 25.7}" -u "Benedyktynska-1" -P "js8Yu*\KTUUsc41\" -i "BENEDYKTYNSKA-1-BD3E875F-BC53-42A4-A257-6B23A3C68944"