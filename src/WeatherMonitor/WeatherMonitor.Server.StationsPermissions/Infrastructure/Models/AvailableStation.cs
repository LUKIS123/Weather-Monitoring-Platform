namespace WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

public readonly record struct AvailableStation(
    int Id,
    string GoogleMapsPlusCode,
    string? DeviceExtraInfo,
    bool IsActive,
    string DeviceName);
