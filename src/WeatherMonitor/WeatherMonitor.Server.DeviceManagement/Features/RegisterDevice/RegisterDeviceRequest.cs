namespace WeatherMonitor.Server.DeviceManagement.Features.RegisterDevice;
internal readonly record struct RegisterDeviceRequest(
    string GoogleMapsPlusCode,
    string DeviceExtraInfo
    );