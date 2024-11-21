using WeatherMonitorCore.Contract.Shared;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Repositories;

namespace WeatherMonitorCore.UserAuthentication.Features.UpdateRole;

internal interface IUpdateRoleService
{
    Task<bool> UpdateRole(string userId, Role role);
}

internal class UpdateRoleService : IUpdateRoleService
{
    private readonly IUserSettingsRepository _userSettingsRepository;

    public UpdateRoleService(IUserSettingsRepository userSettingsRepository)
    {
        _userSettingsRepository = userSettingsRepository;
    }

    public async Task<bool> UpdateRole(string userId, Role role)
    {
        try
        {
            await _userSettingsRepository.SetUserRole(userId, role);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
