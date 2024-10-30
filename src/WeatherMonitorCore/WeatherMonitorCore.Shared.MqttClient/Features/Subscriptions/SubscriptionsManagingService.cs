using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using WeatherMonitorCore.Shared.MqttClient.Features.MqttFactoryHelpers;
using WeatherMonitorCore.Shared.MqttClient.Features.SubscribedTopicsCache;
using WeatherMonitorCore.Shared.MqttClient.Interfaces;

namespace WeatherMonitorCore.Shared.MqttClient.Features.Subscriptions;

internal class SubscriptionsManagingService : ISubscriptionsManagingService
{
    private readonly IMqttFactoryWrapper _mqttFactoryWrapper;
    private readonly ITopicsCache _topicsCache;
    private readonly IMqttClient _mqttClient;
    private readonly ILogger<SubscriptionsManagingService> _logger;

    public SubscriptionsManagingService(
        IMqttFactoryWrapper mqttFactoryWrapper,
        ITopicsCache topicsCache,
        ILogger<SubscriptionsManagingService> logger)
    {
        _mqttFactoryWrapper = mqttFactoryWrapper;
        _topicsCache = topicsCache;
        _logger = logger;
        _mqttClient = mqttFactoryWrapper.CreateMqttClient();
    }

    public IMqttClient GetMqttClient => _mqttClient;

    public async Task AddTopicAsync(string topicToAdd, CancellationToken stoppingToken)
    {
        if (!_topicsCache.TopicsSet.IsEmpty)
        {
            var unsubscribeOptionsBuilder = _mqttFactoryWrapper.CreateUnsubscribeOptionsBuilder();
            foreach (var topic in _topicsCache.TopicsSet)
            {
                unsubscribeOptionsBuilder.WithTopicFilter(topic);
            }

            await _mqttClient.UnsubscribeAsync(unsubscribeOptionsBuilder.Build(), stoppingToken);
        }

        _topicsCache.AddTopic(topicToAdd);
        await SubscribeToTopics(stoppingToken);
    }

    public async Task RemoveTopicAsync(string topicToRemove, CancellationToken stoppingToken)
    {
        if (!_mqttClient.IsConnected)
        {
            _logger.LogWarning(" Failed to unsubscribe from:{topicToRemove}. Mqtt client is not connected", topicToRemove);
            return;
        }

        var unsubscribeOptionsBuilder = _mqttFactoryWrapper.CreateUnsubscribeOptionsBuilder();
        foreach (var topic in _topicsCache.TopicsSet)
        {
            unsubscribeOptionsBuilder.WithTopicFilter(topic);
        }

        await _mqttClient.UnsubscribeAsync(unsubscribeOptionsBuilder.Build(), stoppingToken);

        _topicsCache.RemoveTopic(topicToRemove);
        if (_topicsCache.TopicsSet.IsEmpty)
        {
            return;
        }

        await SubscribeToTopics(stoppingToken);
    }

    public async Task SubscribeToTopics(CancellationToken stoppingToken)
    {
        if (!_mqttClient.IsConnected)
        {
            _logger.LogError("Failed to subscribe to topics");
            return;
        }

        if (_topicsCache.TopicsSet.IsEmpty)
        {
            return;
        }

        var subscribeOptionsBuilder = _mqttFactoryWrapper.CreateSubscribeOptionsBuilder();
        foreach (var topic in _topicsCache.TopicsSet)
        {
            subscribeOptionsBuilder.WithTopicFilter(topic);
        }

        await _mqttClient.SubscribeAsync(subscribeOptionsBuilder.Build(), stoppingToken);
    }

    public async Task SubscribeToTopics(IEnumerable<string> topics, CancellationToken stoppingToken)
    {
        if (!_mqttClient.IsConnected)
        {
            _logger.LogError("Failed to subscribe to topics");
            return;
        }

        if (_topicsCache.TopicsSet.IsEmpty)
        {
            return;
        }

        var subscribeOptionsBuilder = _mqttFactoryWrapper.CreateSubscribeOptionsBuilder();
        _topicsCache.AddTopics(topics);
        foreach (var topic in _topicsCache.TopicsSet)
        {
            subscribeOptionsBuilder.WithTopicFilter(topic);
        }

        await _mqttClient.SubscribeAsync(subscribeOptionsBuilder.Build(), stoppingToken);
    }

    public void Dispose()
    {
        _mqttClient.Dispose();
    }
}
