using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;

namespace WeatherMonitor.Server.UserPermissionManagement.Infrastructure;

public interface IUserManagementRepository
{
    Task<(IEnumerable<PendingPermissionDto> Stations, int totalItems)> GetPendingPermissionRequestsAsync(
        int pageNumber, int pageSize, PermissionStatus pending);

    Task<UserPermissionRequestDto?> GetPermissionRequestAsync(string userId, int deviceId);

    Task<UserPermissionDto?> GetUserPermissionAsync(string userId, int deviceId);

    Task<UserPermissionRequestDto> SetPermissionRequestAsync(
        string userId, int deviceId, PermissionStatus updatePermissionStatus, DateTime zoneAdjustedTime);

    Task<(UserPermissionRequestDto request, UserPermissionDto permission)> AddUserStationPermissionAsync(
        string userId, int updatePermissionDeviceId, PermissionStatus granted, DateTime zoneAdjustedTime);

    Task<UserPermissionRequestDto> RemoveUserStationPermissionAsync(
        string userId, int updatePermissionDeviceId, PermissionStatus denied, DateTime zoneAdjustedTime);

    Task<(IEnumerable<UserDto> users, int totalItems)> GetUsersAsync(
        int pageNumber, int pageSize, string? nicknameSearch = null);

    Task<(IEnumerable<UserPermissionRequestDto> userPermissions, int totalItems)> GetUsersPermissionRequestsAsync(
        int pageNumber, int pageSize, string userId);
}
