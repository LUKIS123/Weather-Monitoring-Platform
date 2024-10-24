namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces;

public interface IMqttEventHandler
{
    Task HandleMqttMessageAsync(string topic, string messagePayload);
}
