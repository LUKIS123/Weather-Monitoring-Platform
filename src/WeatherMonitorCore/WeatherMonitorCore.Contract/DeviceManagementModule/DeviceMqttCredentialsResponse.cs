namespace WeatherMonitorCore.Contract.DeviceManagementModule;

public readonly record struct DeviceMqttCredentialsResponse(
    int Id,
    string Username,
    string Password,
    string ClientId,
    string Topic);
