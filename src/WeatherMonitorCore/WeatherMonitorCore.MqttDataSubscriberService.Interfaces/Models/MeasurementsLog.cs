namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

public readonly record struct MeasurementsLog(
    DateTime ReceivedAt,
    double Humidity,
    double Temperature,
    double AirPressure,
    double Altitude,
    double? PM1_0,
    double? PM2_5,
    double? PM10);
