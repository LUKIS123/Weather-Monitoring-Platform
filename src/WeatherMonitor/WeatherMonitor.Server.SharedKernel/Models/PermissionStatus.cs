namespace WeatherMonitor.Server.SharedKernel.Models;
public enum PermissionStatus
{
    None = 0,
    NotRequested = 1,
    Pending = 2,
    Granted = 3,
    Denied = 4,
}
