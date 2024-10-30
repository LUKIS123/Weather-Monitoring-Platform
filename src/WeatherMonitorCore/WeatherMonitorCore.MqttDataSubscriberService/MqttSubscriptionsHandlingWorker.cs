using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using WeatherMonitorCore.MqttDataSubscriberService.Configuration;
using WeatherMonitorCore.Shared.MqttClient.Interfaces;

namespace WeatherMonitorCore.MqttDataSubscriberService;

public class MqttSubscriptionsHandlingWorker : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly ILogger<MqttSubscriptionsHandlingWorker> _logger;
    private readonly IMqttDataService _mqttDataService;
    private readonly ISubscriptionsManagingService _subscriptionsManagingService;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public MqttSubscriptionsHandlingWorker(
        ILogger<MqttSubscriptionsHandlingWorker> logger,
        IMqttDataService mqttDataService,
        IHostApplicationLifetime hostApplicationLifetime,
        ISubscriptionsManagingService subscriptionsManagingService)
    {
        _logger = logger;
        _mqttDataService = mqttDataService;
        _hostApplicationLifetime = hostApplicationLifetime;
        _subscriptionsManagingService = subscriptionsManagingService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _hostApplicationLifetime.ApplicationStopping.Register(() =>
        {
            Task.Run(async () =>
            {
                await _mqttDataService.CleanUp(WorkerMqttClientConfig.MqttDataServiceGuid, stoppingToken);
            }, stoppingToken);
        });

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Worker started at:{time}", DateTimeOffset.Now);
        }

        await _mqttDataService.HandleMqttSubscriptions(stoppingToken);

        using var timer = new PeriodicTimer(_period);
        var keepAlive = true;
        try
        {
            while (!stoppingToken.IsCancellationRequested && keepAlive)
            {
                keepAlive = await timer.WaitForNextTickAsync(stoppingToken);
                if (_subscriptionsManagingService.GetMqttClient.IsConnected) continue;
                try
                {
                    await _subscriptionsManagingService.GetMqttClient.ReconnectAsync(stoppingToken);
                }
                catch (Exception)
                {
                    _logger.LogWarning("{time}: Cannot reconnect to MQTT", DateTimeOffset.Now);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            await _mqttDataService.CleanUp(WorkerMqttClientConfig.MqttDataServiceGuid, stoppingToken);
        }

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Worker stopped at:{time}", DateTimeOffset.Now);
        }
    }

    public override void Dispose()
    {
        _mqttDataService.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
