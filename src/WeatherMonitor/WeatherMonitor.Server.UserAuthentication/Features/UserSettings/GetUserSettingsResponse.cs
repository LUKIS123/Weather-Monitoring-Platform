using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserAuthentication.Features.UserSettings;
internal class GetUserSettingsResponse
{
    public required string UserName { get; set; }
    public required string PhotoUrl { get; set; }
    public required string Email { get; set; }
    public Role Role { get; set; }
}
