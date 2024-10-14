namespace WeatherMonitor.Server.DeviceManagement.Features.RegisterDevice;
internal readonly record struct RegisterDeviceResponse(
    string GoogleMapsPlusCode,
    string DeviceExtraInfo,
    bool IsActive,
    string MqttUsername // todo
    );