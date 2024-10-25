using WeatherMonitorCore.MqttDataSubscriberService.Configuration;

namespace WeatherMonitorCore.MqttDataSubscriberService;

public class MqttSubscriptionsHandlingWorker : BackgroundService
{
    private readonly ILogger<MqttSubscriptionsHandlingWorker> _logger;
    private readonly IMqttDataService _mqttDataService;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;

    public MqttSubscriptionsHandlingWorker(
        ILogger<MqttSubscriptionsHandlingWorker> logger,
        IMqttDataService mqttDataService,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        _logger = logger;
        _mqttDataService = mqttDataService;
        _hostApplicationLifetime = hostApplicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _hostApplicationLifetime.ApplicationStopping.Register(() =>
        {
            Task.Run(async () =>
            {
                await _mqttDataService.CleanUp(Constants.MqttDataServiceGuid, stoppingToken);
            }, stoppingToken);
        });

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Worker started at:{time}", DateTimeOffset.Now);
        }

        await _mqttDataService.HandleMqttSubscriptions(stoppingToken);

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
