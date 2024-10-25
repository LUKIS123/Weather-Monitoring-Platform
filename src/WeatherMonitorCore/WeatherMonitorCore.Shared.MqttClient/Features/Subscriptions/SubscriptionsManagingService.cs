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

    public SubscriptionsManagingService(IMqttFactoryWrapper mqttFactoryWrapper, ITopicsCache topicsCache)
    {
        _mqttFactoryWrapper = mqttFactoryWrapper;
        _topicsCache = topicsCache;
        _mqttClient = mqttFactoryWrapper.CreateMqttClient();
    }

    public IMqttClient GetMqttClient => _mqttClient;

    public async Task AddTopicAsync(string topicToAdd, CancellationToken stoppingToken)
    {
        var unsubscribeOptionsBuilder = _mqttFactoryWrapper.CreateUnsubscribeOptionsBuilder();
        foreach (var topic in _topicsCache.TopicsSet)
        {
            unsubscribeOptionsBuilder.WithTopicFilter(topic);
        }

        _topicsCache.AddTopic(topicToAdd);
        await _mqttClient.UnsubscribeAsync(unsubscribeOptionsBuilder.Build(), stoppingToken);
        await SubscribeToTopics(stoppingToken);
    }

    public async Task RemoveTopicAsync(string topicToRemove, CancellationToken stoppingToken)
    {
        var unsubscribeOptionsBuilder = _mqttFactoryWrapper.CreateUnsubscribeOptionsBuilder();
        foreach (var topic in _topicsCache.TopicsSet)
        {
            unsubscribeOptionsBuilder.WithTopicFilter(topic);
        }

        _topicsCache.RemoveTopic(topicToRemove);
        await _mqttClient.UnsubscribeAsync(unsubscribeOptionsBuilder.Build(), stoppingToken);
        await SubscribeToTopics(stoppingToken);
    }

    public async Task SubscribeToTopics(CancellationToken stoppingToken)
    {
        var subscribeOptionsBuilder = _mqttFactoryWrapper.CreateSubscribeOptionsBuilder();
        foreach (var topic in _topicsCache.TopicsSet)
        {
            subscribeOptionsBuilder.WithTopicFilter(topic);
        }

        await _mqttClient.SubscribeAsync(subscribeOptionsBuilder.Build(), stoppingToken);
    }

    public async Task SubscribeToTopics(IEnumerable<string> topics, CancellationToken stoppingToken)
    {
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
