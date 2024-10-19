namespace WeatherMonitorCore.Contract.DeviceManagementModule;
public readonly record struct RegisterDeviceRequest(
    string MqttUsername,
    string? GoogleMapsPlusCode,
    string? DeviceExtraInfo
);
