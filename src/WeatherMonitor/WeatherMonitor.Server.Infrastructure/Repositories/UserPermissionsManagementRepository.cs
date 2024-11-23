using Dapper;
using System.Text;
using WeatherMonitor.Server.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;

namespace WeatherMonitor.Server.Infrastructure.Repositories;

internal class UserPermissionsManagementRepository : IUserManagementRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UserPermissionsManagementRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<(IEnumerable<PendingPermissionDto> Stations, int totalItems)> GetPendingPermissionRequestsAsync(
        int pageNumber, int pageSize, PermissionStatus pending)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT
    SP.Id AS {nameof(PendingPermissionDto.Id)},
    SP.UserId AS {nameof(PendingPermissionDto.UserId)},
    SP.DeviceId AS {nameof(PendingPermissionDto.DeviceId)},
    M.Username AS {nameof(PendingPermissionDto.DeviceName)},
    D.GoogleMapsPlusCode AS {nameof(PendingPermissionDto.GoogleMapsPlusCode)},
    SP.Status AS {nameof(PendingPermissionDto.Status)},
    SP.ChangeDate AS {nameof(PendingPermissionDto.RequestedAt)}
FROM [stationsAccess].[StationPermissionRequests] SP
    INNER JOIN [identity].[Devices] D
    ON D.Id = SP.DeviceId 
    LEFT JOIN [identity].[MqttClients] M
    ON M.Id = D.MqttClientId
WHERE SP.Status = @Status
ORDER BY SP.ChangeDate DESC
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY;

SELECT COUNT(*)
FROM [stationsAccess].[StationPermissionRequests]
WHERE Status = @Status;
";
        await using var multi = await connection.QueryMultipleAsync(
            sql,
            new
            {
                OffsetRows = pageSize * (pageNumber - 1),
                FetchRows = pageSize,
                Status = pending
            });
        var permissions = await multi.ReadAsync<PendingPermissionDto>();
        var totalItems = await multi.ReadFirstAsync<int>();

        return (permissions, totalItems);
    }

    public async Task<UserPermissionRequestDto?> GetPermissionRequestAsync(string userId, int deviceId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT TOP 1
    Id AS {nameof(UserPermissionRequestDto.Id)},
    UserId AS {nameof(UserPermissionRequestDto.UserId)},
    DeviceId AS {nameof(UserPermissionRequestDto.DeviceId)},
    Status AS {nameof(UserPermissionRequestDto.PermissionStatus)},
    ChangeDate AS {nameof(UserPermissionRequestDto.DateTime)}
FROM [stationsAccess].[StationPermissionRequests] 
WHERE UserId = @UserId
    AND DeviceId = @DeviceId;
";
        return await connection.QueryFirstOrDefaultAsync(
            sql,
            new
            {
                UserId = userId,
                DeviceId = deviceId
            });
    }

    public async Task<UserPermissionDto?> GetUserPermissionAsync(string userId, int deviceId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT TOP 1
    Id AS {nameof(UserPermissionDto.Id)},
    UserId AS {nameof(UserPermissionDto.UserId)},
    DeviceId AS {nameof(UserPermissionDto.DeviceId)}
  FROM [stationsAccess].[StationsPermissions]
WHERE UserId = @UserId
    AND DeviceId = @DeviceId;
";
        return await connection.QueryFirstOrDefaultAsync(
            sql,
            new
            {
                UserId = userId,
                DeviceId = deviceId
            });
    }

    public async Task<UserPermissionRequestDto> SetPermissionRequestAsync(
        string userId, int deviceId, PermissionStatus updatePermissionStatus, DateTime zoneAdjustedTime)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
MERGE INTO [stationsAccess].[StationPermissionRequests] AS Target
USING (VALUES (
            @UserId, 
            @DeviceId, 
            @Status, 
            @ChangeDate)) 
    AS Source (
            UserId, 
            DeviceId, 
            Status, 
            ChangeDate)
    ON Target.UserId = Source.UserId
       AND Target.DeviceId = Source.DeviceId
WHEN MATCHED THEN
    UPDATE SET 
        Status = Source.Status, 
        ChangeDate = Source.ChangeDate
WHEN NOT MATCHED THEN
    INSERT (
        UserId, 
        DeviceId, 
        Status, 
        ChangeDate)
    VALUES (
        Source.UserId, 
        Source.DeviceId, 
        Source.Status, 
        Source.ChangeDate)
OUTPUT inserted.Id AS {nameof(UserPermissionRequestDto.PermissionStatus)}, 
       inserted.UserId AS {nameof(UserPermissionRequestDto.UserId)}, 
       inserted.DeviceId AS {nameof(UserPermissionRequestDto.DeviceId)}, 
       inserted.Status AS {nameof(UserPermissionRequestDto.PermissionStatus)},
       inserted.ChangeDate AS DateTime AS {nameof(UserPermissionRequestDto.DateTime)};
";
        return await connection.QueryFirstAsync<UserPermissionRequestDto>(sql, new
        {
            UserId = userId,
            DeviceId = deviceId,
            Status = updatePermissionStatus,
            ChangeDate = zoneAdjustedTime
        });
    }

    public async Task<(UserPermissionRequestDto request, UserPermissionDto permission)> AddUserStationPermissionAsync(
        string userId, int updatePermissionDeviceId, PermissionStatus granted, DateTime zoneAdjustedTime)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
BEGIN TRANSACTION;

UPDATE [stationsAccess].[StationPermissionRequests]
SET 
    Status = @GrantedStatus, 
    ChangeDate = @ChangeDate
WHERE UserId = @UserId
    AND DeviceId = @DeviceId;

INSERT INTO [stationsAccess].[StationsPermissions] (
    UserId, 
    DeviceId)
VALUES (
    @UserId, 
    @DeviceId);

SELECT TOP 1
    Id AS {nameof(UserPermissionRequestDto.Id)},
    UserId AS {nameof(UserPermissionRequestDto.UserId)},
    DeviceId AS {nameof(UserPermissionRequestDto.DeviceId)},
    Status AS {nameof(UserPermissionRequestDto.PermissionStatus)},
    ChangeDate AS {nameof(UserPermissionRequestDto.DateTime)}
FROM [stationsAccess].[StationPermissionRequests]
WHERE UserId = @UserId
    AND DeviceId = @DeviceId;

SELECT TOP 1
    Id AS {nameof(UserPermissionDto.Id)},
    UserId AS {nameof(UserPermissionDto.UserId)},
    DeviceId AS {nameof(UserPermissionDto.DeviceId)}
FROM [stationsAccess].[StationsPermissions]
WHERE UserId = @UserId
    AND DeviceId = @DeviceId;

COMMIT TRANSACTION;
";
        await using var multi = await connection.QueryMultipleAsync(sql, new
        {
            UserId = userId,
            DeviceId = updatePermissionDeviceId,
            GrantedStatus = granted,
            ChangeDate = zoneAdjustedTime
        });

        var request = await multi.ReadSingleAsync<UserPermissionRequestDto>();
        var permission = await multi.ReadSingleAsync<UserPermissionDto>();

        return (request, permission);
    }

    public async Task<UserPermissionRequestDto> RemoveUserStationPermissionAsync(
        string userId, int updatePermissionDeviceId, PermissionStatus denied, DateTime zoneAdjustedTime)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
BEGIN TRANSACTION;

DELETE FROM [stationsAccess].[StationsPermissions]
WHERE UserId = @UserId
    AND DeviceId = @DeviceId;

UPDATE [stationsAccess].[StationPermissionRequests]
SET 
    Status = @DeniedStatus, 
    ChangeDate = @ChangeDate
WHERE UserId = @UserId
    AND DeviceId = @DeviceId;

SELECT TOP 1
    Id AS {nameof(UserPermissionRequestDto.Id)},
    UserId AS {nameof(UserPermissionRequestDto.UserId)},
    DeviceId AS {nameof(UserPermissionRequestDto.DeviceId)},
    Status AS {nameof(UserPermissionRequestDto.PermissionStatus)},
    ChangeDate AS {nameof(UserPermissionRequestDto.DateTime)}
FROM [stationsAccess].[StationPermissionRequests]
WHERE UserId = @UserId
    AND DeviceId = @DeviceId;

COMMIT TRANSACTION;
";
        return await connection.QueryFirstOrDefaultAsync<UserPermissionRequestDto>(sql, new
        {
            UserId = userId,
            DeviceId = updatePermissionDeviceId,
            DeniedStatus = denied,
            ChangeDate = zoneAdjustedTime
        });
    }

    public async Task<(IEnumerable<UserDto> users, int totalItems)> GetUsersAsync(int pageNumber, int pageSize, string? nicknameSearch = null)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var sql = new StringBuilder(@$"
SELECT
    Id AS {nameof(UserDto.Id)},
    Role AS {nameof(UserDto.Role)},
    Email AS {nameof(UserDto.Email)},
    Nickname AS {nameof(UserDto.Nickname)}
FROM [identity].[Users]
");
        if (!string.IsNullOrWhiteSpace(nicknameSearch))
        {
            sql.Append(@"
WHERE Nickname LIKE @NicknameSearch + '%'");
        }
        sql.Append(@"
ORDER Nickname
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY;

SELECT COUNT(*)
FROM [identity].[Users]
");
        if (!string.IsNullOrWhiteSpace(nicknameSearch))
        {
            sql.Append(@"
WHERE Nickname LIKE @NicknameSearch + '%'");
        }

        await using var multi = await connection.QueryMultipleAsync(
            sql.ToString(),
            new
            {
                OffsetRows = pageSize * (pageNumber - 1),
                FetchRows = pageSize,
                NicknameSearch = string.IsNullOrWhiteSpace(nicknameSearch)
                    ? (object)DBNull.Value
                    : nicknameSearch
            });
        var users = await multi.ReadAsync<UserDto>();
        var totalItems = await multi.ReadFirstAsync<int>();

        return (users, totalItems);
    }

    public async Task<(IEnumerable<UserPermissionRequestDto> userPermissions, int totalItems)> GetUsersPermissionRequestsAsync(int pageNumber, int pageSize, string userId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT
    SPR.Id AS {nameof(UserRequestWithPermissionDto.Id)},
    SPR.UserId AS {nameof(UserRequestWithPermissionDto.UserId)},
    SPR.DeviceId AS {nameof(UserRequestWithPermissionDto.DeviceId)},
    SPR.Status AS {nameof(UserRequestWithPermissionDto.PermissionStatus)},
    SPR.ChangeDate AS {nameof(UserRequestWithPermissionDto.DateTime)},
    SP.Id AS {nameof(UserRequestWithPermissionDto.PermissionId)},
    SP.UserId AS {nameof(UserRequestWithPermissionDto.PermissionUserId)},
    SP.DeviceId AS {nameof(UserRequestWithPermissionDto.PermissionDeviceId)}
FROM [stationsAccess].[StationPermissionRequests] SPR
LEFT JOIN [stationsAccess].[StationsPermissions] SP
    ON SPR.UserId = SP.UserId AND SPR.DeviceId = SP.DeviceId
WHERE SPR.UserId = @UserId
ORDER BY SPR.ChangeDate DESC
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY;
";
        await using var multi = await connection.QueryMultipleAsync(
            sql,
            new
            {
                OffsetRows = pageSize * (pageNumber - 1),
                FetchRows = pageSize,
                UserId = userId
            });
        var results = await multi.ReadAsync<UserRequestWithPermissionDto>();
        var totalItems = await multi.ReadFirstAsync<int>();

        var permissionRequests = results.Select(rwp =>
            new UserPermissionRequestDto(
                rwp.Id,
                rwp.UserId,
                rwp.DeviceId,
                rwp.PermissionStatus,
                rwp.DateTime));
        return (permissionRequests, totalItems);
    }
}
