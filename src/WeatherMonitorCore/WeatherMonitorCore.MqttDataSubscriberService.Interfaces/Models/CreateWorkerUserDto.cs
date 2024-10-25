namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

public readonly record struct CreateWorkerUserDto(
    Guid Id,
    string Username,
    string Password,
    string ClientId,
    bool IsSuperUser = true);
