using FluentAssertions;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using NSubstitute;
using System.Collections.Immutable;
using WeatherMonitorCore.Shared.MqttClient.Features.MqttFactoryHelpers;
using WeatherMonitorCore.Shared.MqttClient.Features.SubscribedTopicsCache;
using WeatherMonitorCore.Shared.MqttClient.Features.Subscriptions;

namespace WeatherMonitorCore.Shared.MqttClient.Tests.Subscriptions;

[TestFixture]
public class SubscriptionsManagingServiceTests
{
    private IMqttFactoryWrapper _mqttFactoryWrapper = null!;
    private ITopicsCache _topicsCache = null!;
    private IMqttClient _mqttClient = null!;
    private ILogger<SubscriptionsManagingService> _logger = null!;
    private SubscriptionsManagingService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _mqttFactoryWrapper = Substitute.For<IMqttFactoryWrapper>();
        _topicsCache = Substitute.For<ITopicsCache>();
        _mqttClient = Substitute.For<IMqttClient>();
        _logger = Substitute.For<ILogger<SubscriptionsManagingService>>();

        _mqttClient.IsConnected.Returns(true);
        _mqttFactoryWrapper.CreateMqttClient().Returns(_mqttClient);

        _topicsCache.TopicsSet.Returns(new HashSet<string>().ToImmutableHashSet());

        _service = new SubscriptionsManagingService(_mqttFactoryWrapper, _topicsCache, _logger);
    }

    [Test]
    public void Constructor_ShouldCreateMqttClient()
    {
        // Assert
        _mqttFactoryWrapper.Received(1).CreateMqttClient();
        _service.GetMqttClient.Should().Be(_mqttClient);
    }

    [Test]
    public async Task AddTopicAsync_ShouldAddTopicAndSubscribe()
    {
        // Arrange
        var topic = "test/topic";
        var cancellationToken = CancellationToken.None;
        _topicsCache.TopicsSet.Returns(new HashSet<string>().ToImmutableHashSet());

        // Act
        await _service.AddTopicAsync(topic, cancellationToken);

        // Assert
        _topicsCache.Received(1).AddTopic(topic);
    }

    [Test]
    public void Dispose_ShouldDisposeMqttClient()
    {
        // Act
        _service.Dispose();

        // Assert
        _mqttClient.Received(1).Dispose();
    }

    [TearDown]
    public void TearDown()
    {
        _service.Dispose();
        _mqttClient.Dispose();
    }
}