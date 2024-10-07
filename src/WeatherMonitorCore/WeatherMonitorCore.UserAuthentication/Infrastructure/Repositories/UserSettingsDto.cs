using WeatherMonitorCore.Contract.Auth;

namespace WeatherMonitorCore.UserAuthentication.Infrastructure.Repositories;
public record UserSettingsDto(
    string UserId,
    Role Role
);
