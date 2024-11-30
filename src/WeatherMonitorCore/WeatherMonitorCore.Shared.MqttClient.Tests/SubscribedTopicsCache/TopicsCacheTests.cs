using FluentAssertions;
using WeatherMonitorCore.Shared.MqttClient.Features.SubscribedTopicsCache;

namespace WeatherMonitorCore.Shared.MqttClient.Tests.SubscribedTopicsCache;

[TestFixture]
public class TopicsCacheTests
{
    [Test]
    public void Constructor_ShouldInitializeEmptyTopicsSet()
    {
        // Act
        var topicsCache = new TopicsCache();

        // Assert
        topicsCache.TopicsSet.Should().BeEmpty();
    }

    [Test]
    public void Constructor_ShouldInitializeTopicsSetWithGivenList()
    {
        // Arrange
        var topics = new List<string> { "topic1", "topic2", "topic3" };

        // Act
        var topicsCache = new TopicsCache(topics);

        // Assert
        topicsCache.TopicsSet.Should().BeEquivalentTo(topics);
    }

    [Test]
    public void Initialize_ShouldAddTopicsToTopicsSet()
    {
        // Arrange
        var topicsCache = new TopicsCache();
        var topics = new List<string> { "topic1", "topic2" };

        // Act
        topicsCache.Initialize(topics);

        // Assert
        topicsCache.TopicsSet.Should().BeEquivalentTo(topics);
    }

    [Test]
    public void Initialize_ShouldNotDuplicateTopics()
    {
        // Arrange
        var topicsCache = new TopicsCache();
        var topics = new List<string> { "topic1", "topic2", "topic1" };

        // Act
        topicsCache.Initialize(topics);

        // Assert
        topicsCache.TopicsSet.Should().BeEquivalentTo("topic1", "topic2");
    }

    [Test]
    public void AddTopic_ShouldAddTopicToTopicsSet()
    {
        // Arrange
        var topicsCache = new TopicsCache();
        var topicToAdd = "topic1";

        // Act
        var result = topicsCache.AddTopic(topicToAdd);

        // Assert
        result.Should().BeTrue();
        topicsCache.TopicsSet.Should().Contain(topicToAdd);
    }

    [Test]
    public void AddTopic_ShouldReturnFalse_WhenTopicAlreadyExists()
    {
        // Arrange
        var topicsCache = new TopicsCache();
        var topicToAdd = "topic1";
        topicsCache.AddTopic(topicToAdd);

        // Act
        var result = topicsCache.AddTopic(topicToAdd);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void AddTopics_ShouldAddMultipleTopicsToTopicsSet()
    {
        // Arrange
        var topicsCache = new TopicsCache();
        var topicsToAdd = new List<string> { "topic1", "topic2" };

        // Act
        var result = topicsCache.AddTopics(topicsToAdd);

        // Assert
        result.Should().BeTrue();
        topicsCache.TopicsSet.Should().BeEquivalentTo(topicsToAdd);
    }

    [Test]
    public void AddTopics_ShouldReturnFalse_WhenAtLeastOneTopicAlreadyExists()
    {
        // Arrange
        var topicsCache = new TopicsCache(new List<string> { "topic1" });
        var topicsToAdd = new List<string> { "topic1", "topic2" };

        // Act
        var result = topicsCache.AddTopics(topicsToAdd);

        // Assert
        result.Should().BeFalse();
        topicsCache.TopicsSet.Should().BeEquivalentTo("topic1", "topic2");
    }

    [Test]
    public void RemoveTopic_ShouldRemoveTopicFromTopicsSet()
    {
        // Arrange
        var topicsCache = new TopicsCache(new List<string> { "topic1", "topic2" });
        var topicToRemove = "topic1";

        // Act
        topicsCache.RemoveTopic(topicToRemove);

        // Assert
        topicsCache.TopicsSet.Should().NotContain(topicToRemove);
    }

    [Test]
    public void RemoveTopic_ShouldNotThrow_WhenTopicDoesNotExist()
    {
        // Arrange
        var topicsCache = new TopicsCache(new List<string> { "topic1" });
        var topicToRemove = "nonexistent";

        // Act
        topicsCache.Invoking(tc => tc.RemoveTopic(topicToRemove))
            .Should().NotThrow();

        // Assert
        topicsCache.TopicsSet.Should().Contain("topic1");
    }
}
