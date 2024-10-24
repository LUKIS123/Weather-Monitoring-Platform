namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

public readonly record struct CreateSuperUserDto(
    Guid Id,
    string Username,
    string Password,
    string ClientId,
    bool IsSuperUser = true);
