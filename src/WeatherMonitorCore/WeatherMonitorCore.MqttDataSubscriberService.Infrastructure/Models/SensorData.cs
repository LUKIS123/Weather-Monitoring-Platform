namespace WeatherMonitorCore.MqttDataSubscriberService.Infrastructure.Models;

public readonly record struct SensorData
{
    public long Id { get; init; }
    public DateTime MeasuredAt { get; init; }
    public float Humidity { get; init; }
    public float Temperature { get; init; }
    public float AirPressure { get; init; }
    public float? PM1_0 { get; init; }
    public float? PM2_5 { get; init; }
    public float? PM10 { get; init; }
    public int DeviceId { get; init; }
}
