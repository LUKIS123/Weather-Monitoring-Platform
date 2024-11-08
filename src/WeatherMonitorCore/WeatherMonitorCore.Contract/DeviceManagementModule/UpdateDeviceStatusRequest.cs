namespace WeatherMonitorCore.Contract.DeviceManagementModule;

public readonly record struct UpdateDeviceStatusRequest(
    IEnumerable<UpdateDeviceStatus> StationsUpdates);

public readonly record struct UpdateDeviceStatus(
    int DeviceId,
    bool SetActive);
