namespace WeatherMonitorCore.MqttAuth.Infrastructure.Models;

public readonly record struct BrokerClientAuthenticationDto(
    string Username,
    string Password,
    string ClientId);
