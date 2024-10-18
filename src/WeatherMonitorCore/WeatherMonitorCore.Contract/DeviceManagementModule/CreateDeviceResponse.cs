namespace WeatherMonitorCore.Contract.DeviceManagementModule;

public readonly record struct CreateDeviceResponse(
    int Id,
    string? GoogleMapsPlusCode,
    string? DeviceExtraInfo,
    bool IsActivate,
    string Username,
    string Password,
    string ClientId,
    string Topic
    );
