namespace WeatherMonitorCore.DeviceManagement.Infrastructure.Models;

public readonly record struct MqttCredentialsDto(
    int Id,
    string Username,
    string Password,
    string ClientId,
    string Topic);
