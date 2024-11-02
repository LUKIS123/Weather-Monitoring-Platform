namespace WeatherMonitor.Server.DataView.Infrastructure.Models;

public readonly record struct GetStationResponse(
    string Username,
    int DeviceId,
    string GoogleMapsPlusCode,
    string? DeviceExtraInfo,
    bool IsActive,
    double? Temperature,
    double? Humidity,
    double? AirPressure,
    double? Altitude,
    double? PM10,
    double? PM1_0,
    double? PM2_5,
    DateTime? ReceivedAt);
