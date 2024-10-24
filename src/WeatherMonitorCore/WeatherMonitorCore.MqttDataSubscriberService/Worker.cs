namespace WeatherMonitorCore.MqttDataSubscriberService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMqttDataService _mqttDataService;

    public Worker(ILogger<Worker> logger, IMqttDataService mqttDataService)
    {
        _logger = logger;
        _mqttDataService = mqttDataService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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
    }
}
