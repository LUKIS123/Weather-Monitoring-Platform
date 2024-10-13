using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserAuthentication.Infrastructure.Models;
public class UserSettingsDto
{
    public required string UserName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Email { get; set; }
    public Role Role { get; set; }
}
