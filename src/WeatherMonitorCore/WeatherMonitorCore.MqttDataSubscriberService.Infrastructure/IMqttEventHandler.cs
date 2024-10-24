namespace WeatherMonitorCore.MqttDataSubscriberService.Infrastructure;
public interface IMqttEventHandler
{
    Task HandleMqttMessageAsync(string topic, string messagePayload);
}
