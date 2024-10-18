namespace WeatherMonitor.Server.DeviceManagement.Features.RegisterDevice;
internal readonly record struct RegisterDeviceResponse(
    int Id,
    string? GoogleMapsPlusCode,
    string? DeviceExtraInfo,
    bool IsActivate,
    string Username,
    string Password,
    string ClientId,
    string Topic
    );