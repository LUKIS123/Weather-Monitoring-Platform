using Microsoft.Extensions.Logging;
using System.Text.Json;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

namespace WeatherMonitorCore.Infrastructure.Utility;

internal interface IDeviceMqttMessageParser
{
    SensorData ParseMessage(string payload);
}

internal class DeviceMqttMessageParser
{
    private readonly ILogger<DeviceMqttMessageParser> _logger;

    public DeviceMqttMessageParser(ILogger<DeviceMqttMessageParser> logger)
    {
        _logger = logger;
    }

    public SensorData ParseMessage(string payload)
    {
        try
        {
            return JsonSerializer.Deserialize<SensorData>(payload);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while parsing message");
            return default;
        }
    }
}
