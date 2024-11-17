using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitorCore.Contract.UserAuthenticationModule;
public class UpdateRoleRequest
{
    public required string UserId { get; set; }
    public Role Role { get; set; }
}
