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
        try
        {
            await Task.Delay(1000, stoppingToken);
            _logger.LogInformation("Worker started at:{time}", DateTimeOffset.Now);

            await _mqttDataService.HandleMqttSubscriptions(stoppingToken);

            using var timer = new PeriodicTimer(_period);
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {

                if (_subscriptionsManagingService.GetMqttClient.IsConnected) continue;
                try
                {
                    await _subscriptionsManagingService.GetMqttClient.ReconnectAsync(stoppingToken);
                    _logger.LogInformation("Mqtt client reconnected at:{time}", DateTimeOffset.Now);

                    await _mqttDataService.ReSubscribeTopics(stoppingToken);
                    _logger.LogInformation("Mqtt client resubscribed to topics at:{time}", DateTimeOffset.Now);
                }
                catch (Exception)
                {
                    _logger.LogWarning("{time}: Cannot reconnect to MQTT", DateTimeOffset.Now);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{time}: Error occurred in the worker", DateTimeOffset.Now);
            _hostApplicationLifetime.StopApplication();
        }
        finally
        {
            await _mqttDataService.CleanUp(WorkerMqttClientConfig.MqttDataServiceGuid, stoppingToken);

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
