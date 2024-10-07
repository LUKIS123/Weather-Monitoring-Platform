using WeatherMonitorCore.Contract.Auth;

namespace WeatherMonitor.Server.UserAuthorization.Infrastructure.Models;
public class BasicUserAuthorizationDto
{
    public required string Id { get; set; }
    public required Role Role { get; set; }
}
