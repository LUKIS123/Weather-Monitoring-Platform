using Microsoft.Extensions.Logging;
using WeatherMonitorCore.Infrastructure.Utility;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;

namespace WeatherMonitorCore.Infrastructure.MqttEventHandlers;
internal class WeatherDataSavingHandler : IMqttEventHandler
{
    private readonly ILogger<WeatherDataSavingHandler> _logger;
    private readonly ISensorDataRepository _sensorDataRepository;
    private readonly IDeviceMqttMessageParser _deviceMqttMessageParser;
    private readonly TimeProvider _timeProvider;
    private readonly ITimeZoneProvider _timeZoneProvider;

    public WeatherDataSavingHandler(
        ISensorDataRepository sensorDataRepository,
        IDeviceMqttMessageParser deviceMqttMessageParser,
        TimeProvider timeProvider,
        ITimeZoneProvider timeZoneProvider,
        ILogger<WeatherDataSavingHandler> logger)
    {
        _sensorDataRepository = sensorDataRepository;
        _deviceMqttMessageParser = deviceMqttMessageParser;
        _timeProvider = timeProvider;
        _timeZoneProvider = timeZoneProvider;
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
            var zoneAdjustedTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(_timeProvider.GetUtcNow().DateTime, _timeZoneProvider.GetTimeZoneInfo());

            var measurementsLog = new MeasurementsLog(
                zoneAdjustedTimeStamp,
                sensorData.Humidity,
                sensorData.Temperature,
                sensorData.AirPressure,
                sensorData.Altitude,
                sensorData.PM1_0,
                sensorData.PM2_5,
                sensorData.PM10);

            await _sensorDataRepository.AddSensorDataAsync(measurementsLog, topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling mqtt message");
        }
    }
}
