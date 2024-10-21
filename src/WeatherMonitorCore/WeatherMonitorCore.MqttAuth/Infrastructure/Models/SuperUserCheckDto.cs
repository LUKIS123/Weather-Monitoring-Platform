namespace WeatherMonitorCore.MqttAuth.Infrastructure.Models;

public readonly record struct SuperUserCheckDto(
    string Username,
    bool IsSuperUser);
