using WeatherMonitorCore.Contract.Auth;

namespace WeatherMonitorCore.UserAuthentication.Infrastructure.Repositories;
public interface IUserSettingsRepository
{
    Task<UserSettingsDto> GetOrCreateUser(string userId, Role role);
    Task<UserSettingsDto> SetUserRole(string userId, Role role);
}
