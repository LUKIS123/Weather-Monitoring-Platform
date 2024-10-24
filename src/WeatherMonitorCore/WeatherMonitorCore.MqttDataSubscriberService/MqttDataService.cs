using MQTTnet;
using MQTTnet.Client;
using System.Text;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;
using WeatherMonitorCore.MqttDataSubscriberService.Utils;

namespace WeatherMonitorCore.MqttDataSubscriberService;

public interface IMqttDataService : IDisposable
{
    Task HandleMqttSubscriptions(CancellationToken stoppingToken);
}

internal class MqttDataService : IMqttDataService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly IAppMqttClientsRepository _mqttClientsRepository;

    private readonly IEnumerable<IMqttEventHandler> _mqttEventHandlers;// TODO handlery i bedziemy dodawac je on message received
    private readonly MqttFactory _mqttFactory;
    private readonly IMqttClient _mqttClient;
    private MqttClientOptions _mqttClientOptions;
    private readonly HashSet<string> _topicsSet = ["weather/627882BB-35F2-4D56-95A5-78E37C0DA3FC", "weather/EDA796F1-9ECD-4874-BF7E-AA1CE8392FFD"];// TODO: wyniesc metody z subskrybcjami do innej paczki, moze do subscriberservice.interfaces a impl w infrze
    private readonly object _lock = new();

    public MqttDataService(
        // IEnumerable<IMqttEventHandler> mqttEventHandlers
        IAppMqttClientsRepository mqttClientsRepository
        )
    {
        _mqttClientsRepository = mqttClientsRepository;

        // _mqttEventHandlers = mqttEventHandlers;

        _mqttFactory = new MqttFactory();
        _mqttClient = _mqttFactory.CreateMqttClient();

    }

    public async Task HandleMqttSubscriptions(CancellationToken stoppingToken)
    {
        var gen = new ServiceWorkerMqttClientGenerator();
        var cl = gen.GenerateSuperUserCredentials();
        await _mqttClientsRepository.CreateSuperUserAsync(new CreateWorkerUserDto(cl.Id, cl.Username, cl.Password, cl.ClientId, cl.IsSuperUser));

        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithClientId(cl.ClientId)
            .WithCredentials(cl.Username, cl.Password)
            // .WithTlsOptions(new MqttClientTlsOptions
            // {
            //     UseTls = true,
            //     CertificateValidationHandler = context => true,
            //     AllowUntrustedCertificates = true,
            //     IgnoreCertificateChainErrors = true,
            //     IgnoreCertificateRevocationErrors = true
            // })
            .Build();


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

        await _mqttClient.SubscribeAsync(subscribeOptionsBuilder.Build(), stoppingToken);
    }

    public void Dispose()
    {
        _mqttClient.Dispose();
    }
}


// 1 utworzc super usera i wpisac credentialsy (zaciag z bazy)
// 2 przetestowac cz dziala na example userze
// 3 refactor. zrobic z tego serwis, dodac eventy, dodac obsluge eventow
// interface moga byc w blizniaczej paczce interfaces ale implementacja w infrastrukturze