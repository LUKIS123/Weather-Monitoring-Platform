using WeatherMonitorCore.MqttDataSubscriberService.Interfaces;

namespace WeatherMonitorCore.Infrastructure.MqttEventHandlers;
internal class WeatherDataSavingHandler : IMqttEventHandler
{
    public Task HandleMqttMessageAsync(string topic, string messagePayload)
    {
        Console.WriteLine($"Received message: {messagePayload} on topic {topic}");
        return Task.CompletedTask;
    }
}
