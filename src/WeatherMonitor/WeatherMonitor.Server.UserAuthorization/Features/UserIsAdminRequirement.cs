using Microsoft.AspNetCore.Authorization;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserAuthorization.Features;
internal class UserIsAdminRequirement : IAuthorizationRequirement
{
    public IEnumerable<Role> AdminRoleTypes { get; }
    public string UserIdClaimTypeName { get; }
    public string RoleClaimTypeName { get; }

    public UserIsAdminRequirement(IEnumerable<Role> adminRoleTypes, string userIdClaimTypeName, string roleClaimTypeName)
    {
        AdminRoleTypes = adminRoleTypes;
        UserIdClaimTypeName = userIdClaimTypeName;
        RoleClaimTypeName = roleClaimTypeName;
    }
}
