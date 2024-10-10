using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitorCore.UserAuthentication.Infrastructure.Repositories;
public record UserSettingsDto(
    string UserId,
    Role Role
);
