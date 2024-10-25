using Microsoft.Extensions.Logging;
using WeatherMonitorCore.Infrastructure.Utility;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;

namespace WeatherMonitorCore.Infrastructure.MqttEventHandlers;
internal class WeatherDataSavingHandler : IMqttEventHandler
{
    private readonly ILogger<WeatherDataSavingHandler> _logger;
    private readonly ISensorDataRepository _sensorDataRepository;
    private readonly IDeviceMqttMessageParser _deviceMqttMessageParser;
    private readonly TimeProvider _timeProvider;

    public WeatherDataSavingHandler(
        ISensorDataRepository sensorDataRepository,
        IDeviceMqttMessageParser deviceMqttMessageParser,
        TimeProvider timeProvider,
        ILogger<WeatherDataSavingHandler> logger)
    {
        _sensorDataRepository = sensorDataRepository;
        _deviceMqttMessageParser = deviceMqttMessageParser;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task HandleMqttMessageAsync(string topic, string messagePayload)
    {
        if (string.IsNullOrWhiteSpace(topic))
        {
            return;
        }

        try
        {
            var sensorData = _deviceMqttMessageParser.ParseMessage(messagePayload);
            if (sensorData.Equals(default))
            {
                sensorData = sensorData with
                {
                    MeasuredAt = _timeProvider.GetLocalNow().LocalDateTime
                };
            }

            await _sensorDataRepository.AddSensorDataAsync(sensorData, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling mqtt message");
        }
    }
}
