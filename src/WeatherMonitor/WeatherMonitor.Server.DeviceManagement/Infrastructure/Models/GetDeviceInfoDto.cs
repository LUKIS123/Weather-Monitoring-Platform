namespace WeatherMonitor.Server.DeviceManagement.Infrastructure.Models;

public readonly record struct GetDeviceInfoDto(
    int Id,
    string GoogleMapsPlusCode,
    bool IsActive,
    Guid MqttClientId,
    string MqttUsername,
    string MqttBrokerClientId,
    bool IsSuperUser
    );
