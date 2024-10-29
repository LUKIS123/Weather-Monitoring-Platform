namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

public readonly record struct SensorData
{
    public DateTime MeasuredAt { get; init; }
    public float Humidity { get; init; }
    public float Temperature { get; init; }
    public float AirPressure { get; init; }
    public float Altitude { get; init; }
    public float? PM1_0 { get; init; }
    public float? PM2_5 { get; init; }
    public float? PM10 { get; init; }
}
