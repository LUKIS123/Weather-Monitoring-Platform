namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

public readonly record struct SensorData
{
    public double Humidity { get; init; }
    public double Temperature { get; init; }
    public double AirPressure { get; init; }
    public double Altitude { get; init; }
    public double? PM1_0 { get; init; }
    public double? PM2_5 { get; init; }
    public double? PM10 { get; init; }
}
