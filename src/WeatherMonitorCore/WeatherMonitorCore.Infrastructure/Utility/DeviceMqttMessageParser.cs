using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

namespace WeatherMonitorCore.Infrastructure.Utility;

internal interface IDeviceMqttMessageParser
{
    SensorData ParseMessage(string payload);
}

internal partial class DeviceMqttMessageParser : IDeviceMqttMessageParser
{
    [GeneratedRegex(@"([{,]\s*)([A-Za-z0-9_]+)\s*:")]
    private static partial Regex PropertyNameRegex();
    [GeneratedRegex(@":\s*([^,{}]+)")]
    private static partial Regex PropertyValueRegex();

    private readonly ILogger<DeviceMqttMessageParser> _logger;

    public DeviceMqttMessageParser(ILogger<DeviceMqttMessageParser> logger)
    {
        _logger = logger;
    }

    public SensorData ParseMessage(string payload)
    {
        payload = FixJsonString(payload);

        try
        {
            var data = JsonConvert.DeserializeObject<SensorData>(payload);

            if (!data.Equals(default))
            {
                return data;
            }

            _logger.LogError("Deserialization returned null. Payload:{payload}", payload);
            return default;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error while parsing message:{payload}", payload);
            return default;
        }
    }

    private static string FixJsonString(string json)
    {
        json = json.Trim();
        if (json.StartsWith('\"') && json.EndsWith('\"'))
        {
            json = json.Substring(1, json.Length - 2);
        }

        json = PropertyNameRegex().Replace(json, "$1\"$2\":");
        json = PropertyValueRegex().Replace(json, m =>
        {
            var value = m.Groups[1].Value.Trim();
            if (double.TryParse(value, out _) || bool.TryParse(value, out _) || value == "null")
            {
                return $": {value}";
            }

            return $": \"{value}\"";
        });

        return json;
    }
}
