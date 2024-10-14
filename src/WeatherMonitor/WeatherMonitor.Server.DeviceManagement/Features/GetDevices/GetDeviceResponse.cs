namespace WeatherMonitor.Server.DeviceManagement.Features.GetDevices;
internal readonly record struct GetDeviceResponse(
    string GoogleMapsPlusCode,
    bool IsActive,
    string MqttUsername,
    string MqttClientId
);