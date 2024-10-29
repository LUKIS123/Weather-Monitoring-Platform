namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

public readonly record struct MeasurementsLog(
    DateTime ReceivedAt,
    float Humidity,
    float Temperature,
    float AirPressure,
    float Altitude,
    float? PM1_0,
    float? PM2_5,
    float? PM10);
