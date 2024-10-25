using System.Collections.Immutable;

namespace WeatherMonitorCore.Shared.MqttClient.Features.SubscribedTopicsCache;

internal interface ITopicsCache
{
    ImmutableHashSet<string> TopicsSet { get; }
    void Initialize(List<string> topics);
    bool AddTopic(string topicToAdd);
    bool AddTopics(IEnumerable<string> topicsToAdd);
    void RemoveTopic(string topicToRemove);
}

internal class TopicsCache : ITopicsCache
{
    private readonly HashSet<string> _topicsSet = [];
    private readonly object _lock = new();

    public TopicsCache()
    {
    }

    public TopicsCache(List<string> topics)
    {
        _topicsSet = [.. topics];
    }

    public ImmutableHashSet<string> TopicsSet
    {
        get
        {
            lock (_lock)
            {
                return _topicsSet.ToImmutableHashSet();
            }
        }
    }

    public void Initialize(List<string> topics)
    {
        lock (_lock)
        {
            foreach (var topic in topics)
            {
                _topicsSet.Add(topic);
            }
        }
    }

    public bool AddTopic(string topicToAdd)
    {
        bool result;
        lock (_lock)
        {
            result = _topicsSet.Add(topicToAdd);
        }
        return result;
    }

    public bool AddTopics(IEnumerable<string> topicsToAdd)
    {
        var result = true;
        lock (_lock)
        {
            foreach (var topic in topicsToAdd)
            {
                if (!_topicsSet.Add(topic))
                {
                    result = false;
                }
            }
        }
        return result;
    }

    public void RemoveTopic(string topicToRemove)
    {
        lock (_lock)
        {
            _topicsSet.Remove(topicToRemove);
        }
    }
}
