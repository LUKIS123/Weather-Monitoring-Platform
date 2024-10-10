using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitorCore.UserAuthentication.Infrastructure.Repositories;
public interface IUserSettingsRepository
{
    Task<UserSettingsDto?> GetUser(string userId);
    Task<UserSettingsDto> GetOrCreateUser(string userId, Role role = Role.User);
    Task SetUserRole(string userId, Role role);
}
