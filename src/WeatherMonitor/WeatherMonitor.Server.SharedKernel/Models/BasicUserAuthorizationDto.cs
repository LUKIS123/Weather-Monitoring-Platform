using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.SharedKernel.Models;
public class BasicUserAuthorizationDto
{
    public required string Id { get; set; }
    public required Role Role { get; set; }
}
