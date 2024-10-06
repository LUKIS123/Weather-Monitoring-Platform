using WeatherMonitorCore.SharedKernel.Models;

namespace WeatherMonitorCore.UserAuthorization.Infrastructure.Models;

public class BasicUserAuthorizationDto
{
    public required string Id { get; set; }
    public required Role Role { get; set; }
}