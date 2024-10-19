namespace WeatherMonitor.Server.DeviceManagement.Features.GetDevices;
internal readonly record struct GetDeviceResponse(
    int Id,
    string? GoogleMapsPlusCode,
    bool IsActive,
    string? DeviceExtraInfo,
    string MqttUsername,
    string MqttBrokerClientId
);