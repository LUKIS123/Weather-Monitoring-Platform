using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using System.Text;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.MqttDataSubscriberService.Configuration;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;
using WeatherMonitorCore.MqttDataSubscriberService.Utils;
using WeatherMonitorCore.Shared.MqttClient.Interfaces;

namespace WeatherMonitorCore.MqttDataSubscriberService;

public interface IMqttDataService : IDisposable
{
    MqttClientOptions GetMqttClientOptions();
    Task HandleMqttSubscriptions(CancellationToken stoppingToken);
    Task CleanUp(Guid clientId, CancellationToken stoppingToken);
    Task ReSubscribeTopics(CancellationToken stoppingToken);
}

internal class MqttDataService : IMqttDataService
{
    private readonly ILogger<MqttDataService> _logger;
    private readonly IAppMqttClientsRepository _mqttClientsRepository;
    private readonly IEnumerable<IMqttEventHandler> _mqttEventHandlers;
    private readonly ISubscriptionsManagingService _subscriptionsManagingService;
    private readonly IServiceWorkerMqttClientGenerator _serviceWorkerMqttClientGenerator;
    private readonly IAesEncryptionHelper _aesEncryptionHelper;
    private readonly MqttBrokerConnection _brokerConnection;
    private MqttClientOptions _mqttClientOptions = new();

    public MqttDataService(
        IEnumerable<IMqttEventHandler> mqttEventHandlers,
        IAppMqttClientsRepository mqttClientsRepository,
        ISubscriptionsManagingService subscriptionsManagingService,
        IServiceWorkerMqttClientGenerator serviceWorkerMqttClientGenerator,
        IAesEncryptionHelper aesEncryptionHelper,
        MqttBrokerConnection brokerConnection,
        ILogger<MqttDataService> logger)
    {
        _mqttEventHandlers = mqttEventHandlers;
        _mqttClientsRepository = mqttClientsRepository;
        _subscriptionsManagingService = subscriptionsManagingService;
        _serviceWorkerMqttClientGenerator = serviceWorkerMqttClientGenerator;
        _aesEncryptionHelper = aesEncryptionHelper;
        _brokerConnection = brokerConnection;
        _logger = logger;
    }

    public MqttClientOptions GetMqttClientOptions() => _mqttClientOptions;

    public async Task HandleMqttSubscriptions(CancellationToken stoppingToken)
    {
        var superUserCredentials = _serviceWorkerMqttClientGenerator.GenerateSuperUserCredentials();
        await _mqttClientsRepository.CreateSuperUserAsync(
            new CreateWorkerUserDto(
                superUserCredentials.Id,
                superUserCredentials.Username,
                _aesEncryptionHelper.Encrypt(superUserCredentials.Password),
                superUserCredentials.ClientId,
                superUserCredentials.IsSuperUser));

        _mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_brokerConnection.Host, _brokerConnection.Port)
            .WithClientId(superUserCredentials.ClientId)
            .WithCredentials(superUserCredentials.Username, superUserCredentials.Password)
            .Build();

        foreach (var mqttEventHandler in _mqttEventHandlers)
        {
            _subscriptionsManagingService.GetMqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var topic = e.ApplicationMessage.Topic ?? string.Empty;
                var messagePayload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);

                await mqttEventHandler.HandleMqttMessageAsync(topic, messagePayload);
            };
        }

        await _subscriptionsManagingService.GetMqttClient.ConnectAsync(_mqttClientOptions, stoppingToken);

        _logger.LogInformation("Mqtt client connected at:{time}", DateTimeOffset.Now);

        var devicesTopics = await _mqttClientsRepository.GetDevicesTopicsAsync();

        await _subscriptionsManagingService.SubscribeToTopics(
            devicesTopics.Select(i => i.Topic),
            stoppingToken);

        _logger.LogInformation("Mqtt client subscribed to topics at:{time}", DateTimeOffset.Now);
    }

    public async Task ReSubscribeTopics(CancellationToken stoppingToken)
    {
        var devicesTopics = await _mqttClientsRepository.GetDevicesTopicsAsync();

        await _subscriptionsManagingService.SubscribeToTopics(
            devicesTopics.Select(i => i.Topic),
            stoppingToken);
    }

    public async Task CleanUp(Guid clientId, CancellationToken stoppingToken)
    {
        try
        {
            await _subscriptionsManagingService.GetMqttClient.DisconnectAsync(new MqttClientDisconnectOptions(), stoppingToken);
            await _mqttClientsRepository.RemoveWorkerUserAsync(clientId);
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void Dispose()
    {
        _subscriptionsManagingService.Dispose();
    }
}

// _mqttClientOptions = new MqttClientOptionsBuilder()
//     .WithTcpServer(_brokerConnection.Host, _brokerConnection.Port)
//     .WithClientId(superUserCredentials.ClientId)
//     .WithCredentials(superUserCredentials.Username, superUserCredentials.Password)
//     .WithTlsOptions(new MqttClientTlsOptions
//     {
//         UseTls = true,
//         CertificateValidationHandler = context => true,
//         AllowUntrustedCertificates = true,
//         IgnoreCertificateChainErrors = true,
//         IgnoreCertificateRevocationErrors = true
//     })
//     .Build();