using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.Interfaces;
public interface IUserAccessor
{
    string? UserId { get; }
    string? UserName { get; }
    string? PhotoUrl { get; }
    string? Email { get; }
    Role Role { get; }
    string? Token { get; }
}
