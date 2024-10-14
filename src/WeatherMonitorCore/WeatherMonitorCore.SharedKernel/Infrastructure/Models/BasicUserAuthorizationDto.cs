using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitorCore.SharedKernel.Infrastructure.Models;
public class BasicUserAuthorizationDto
{
    public required string Id { get; set; }
    public required Role Role { get; set; }
}
