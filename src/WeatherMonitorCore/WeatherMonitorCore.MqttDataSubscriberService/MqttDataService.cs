using MQTTnet;
using MQTTnet.Client;
using System.Text;
using WeatherMonitorCore.MqttDataSubscriberService.Infrastructure;

namespace WeatherMonitorCore.MqttDataSubscriberService;

public interface IMqttDataService : IDisposable
{
    Task HandleMqttSubscriptions(CancellationToken stoppingToken);
}

internal class MqttDataService : IMqttDataService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly IEnumerable<IMqttEventHandler> _mqttEventHandlers;// TODO handlery i bedziemy dodawac je on message received
    private readonly MqttFactory _mqttFactory;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttClientOptions;
    private readonly HashSet<string> _topicsSet = [];// TODO: wyniesc metody z subskrybcjami do innej paczki
    private readonly object _lock = new();
    private MqttClientSubscribeOptions _subscribeOptions;


    public MqttDataService(IEnumerable<IMqttEventHandler> mqttEventHandlers)
    {
        _mqttEventHandlers = mqttEventHandlers;

        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();
        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("broker.hivemq.com")
            .WithClientId("")
            .WithCredentials("", "")
            // .WithTlsOptions(new MqttClientTlsOptions()
            // {
            //     UseTls = true
            // })
            .Build();
    }

    public async Task HandleMqttSubscriptions(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);

        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var messagePayload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

            Console.WriteLine($"Received message: {messagePayload} on topic {topic}");
            return Task.CompletedTask;
        };

        await _mqttClient.ConnectAsync(_mqttClientOptions, stoppingToken);
        await SubscribeToTopics(_topicsSet, stoppingToken);

        var keepAlive = true;
        while (!stoppingToken.IsCancellationRequested && keepAlive)
        {
            keepAlive = await timer.WaitForNextTickAsync(stoppingToken);
        }

        await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions(), stoppingToken);
    }

    public async Task AddTopicAsync(string topicToAdd, CancellationToken stoppingToken)
    {
        var unsubscribeOptionsBuilder = _mqttFactory.CreateUnsubscribeOptionsBuilder();
        lock (_lock)
        {
            foreach (var topic in _topicsSet)
            {
                unsubscribeOptionsBuilder.WithTopicFilter(topic);
            }

            _topicsSet.Add(topicToAdd);
        }

        await _mqttClient.UnsubscribeAsync(unsubscribeOptionsBuilder.Build(), stoppingToken);
        await SubscribeToTopics(_topicsSet, stoppingToken);
    }

    private async Task SubscribeToTopics(IEnumerable<string> topics, CancellationToken stoppingToken)
    {
        //     var mqttTopicFilterBuilder = new MqttTopicFilterBuilder();
        //     foreach (var topic in _topicsBag)
        //     {
        //         mqttTopicFilterBuilder.WithTopic(topic);
        //     }
        //
        // private MqttTopicFilter _mqttTopicFilter;
        // _mqttTopicFilter = mqttTopicFilterBuilder.Build();
        //     await _mqttClient.SubscribeAsync(_mqttTopicFilter, stoppingToken);

        var subscribeOptionsBuilder = _mqttFactory.CreateSubscribeOptionsBuilder();
        lock (_lock)
        {
            foreach (var topic in topics)
            {
                subscribeOptionsBuilder.WithTopicFilter(topic);
            }
        }

        _subscribeOptions = subscribeOptionsBuilder.Build();
        await _mqttClient.SubscribeAsync(_subscribeOptions, stoppingToken);
    }

    public void Dispose()
    {
        _mqttClient.Dispose();
    }
}
