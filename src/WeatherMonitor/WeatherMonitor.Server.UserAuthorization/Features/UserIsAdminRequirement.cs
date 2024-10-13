using Microsoft.AspNetCore.Authorization;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserAuthorization.Features;
internal class UserIsAdminRequirement : IAuthorizationRequirement
{
    public IEnumerable<Role> AdminRoleTypes { get; }
    public string UserIdClaimTypeName { get; }

    public UserIsAdminRequirement(IEnumerable<Role> adminRoleTypes, string userIdClaimTypeName)
    {
        AdminRoleTypes = adminRoleTypes;
        UserIdClaimTypeName = userIdClaimTypeName;
    }
}
