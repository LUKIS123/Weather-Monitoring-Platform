namespace WeatherMonitor.Server.DeviceManagement.Infrastructure.Models;

public readonly record struct GetDeviceInfoDto(
    int Id,
    string GoogleMapsPlusCode,
    bool IsActive,
    string? DeviceExtraInfo,
    Guid MqttClientId,
    string MqttUsername,
    string Password,
    string MqttBrokerClientId,
    bool IsSuperUser
    );
