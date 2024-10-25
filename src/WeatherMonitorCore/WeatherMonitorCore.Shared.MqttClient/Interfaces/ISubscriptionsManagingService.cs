using MQTTnet.Client;

namespace WeatherMonitorCore.Shared.MqttClient.Interfaces;

public interface ISubscriptionsManagingService : IDisposable
{
    IMqttClient GetMqttClient { get; }
    Task AddTopicAsync(string topicToAdd, CancellationToken stoppingToken);
    Task RemoveTopicAsync(string topicToRemove, CancellationToken stoppingToken);
    Task SubscribeToTopics(CancellationToken stoppingToken);
    Task SubscribeToTopics(IEnumerable<string> topics, CancellationToken stoppingToken);
}