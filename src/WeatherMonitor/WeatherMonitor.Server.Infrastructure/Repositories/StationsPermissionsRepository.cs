using Dapper;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

namespace WeatherMonitor.Server.Infrastructure.Repositories;
internal class StationsPermissionsRepository : IStationsPermissionsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public StationsPermissionsRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<(IEnumerable<AvailableStation> Stations, int totalItems)> GetAvailableStationsAsync(int pageNumber, int pageSize)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT D.Id AS {nameof(AvailableStation.Id)},
    D.GoogleMapsPlusCode AS {nameof(AvailableStation.GoogleMapsPlusCode)},
    D.DeviceExtraInfo AS {nameof(AvailableStation.DeviceExtraInfo)},
    D.IsActive AS {nameof(AvailableStation.IsActive)},
    C.Username AS {nameof(AvailableStation.DeviceName)}
FROM [identity].[Devices] D
    LEFT JOIN [identity].[MqttClients] C
    ON D.MqttClientId = C.Id
WHERE D.IsActive = 1
ORDER BY D.Id
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY;

SELECT COUNT(*)
FROM [identity].[Devices]
WHERE IsActive = 1;
";
        await using var multi = await connection.QueryMultipleAsync(
            sql,
            new
            {
                OffsetRows = pageSize * (pageNumber - 1),
                FetchRows = pageSize
            });
        var availableStations = await multi.ReadAsync<AvailableStation>();
        var totalItems = await multi.ReadFirstAsync<int>();

        return (availableStations, totalItems);
    }

    public async Task<StationUserPermissionDto> GetStationPermissionStatusAsync(int stationId, string userId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
WITH WeatherDevices AS (
    SELECT
        D.Id AS DeviceId,
        M.Username AS MqttUsername
    FROM [identity].[Devices] D
    INNER JOIN [identity].[MqttClients] M
        ON M.Id = D.MqttClientId
    WHERE D.Id = @stationId
)
SELECT
    WD.DeviceId AS {nameof(StationUserPermissionDto.StationId)},
    WD.MqttUsername AS {nameof(StationUserPermissionDto.StationName)},
    U.Id AS {nameof(StationUserPermissionDto.UserId)},
    U.Role AS {nameof(StationUserPermissionDto.UserRole)},
    PR.Status AS {nameof(StationUserPermissionDto.PermissionStatus)},
    PR.ChangeDate AS {nameof(StationUserPermissionDto.ChangeDate)},
    CASE 
        WHEN EXISTS (
            SELECT TOP 1 [Id]
            FROM [stationsAccess].[StationPermissionRequests]
            WHERE UserId = @userId AND DeviceId = @stationId
        ) THEN 1
        ELSE 0
    END AS {nameof(StationUserPermissionDto.PermissionRecordExists)}
FROM [identity].[Users] U
    LEFT JOIN [stationsAccess].[StationPermissionRequests] PR
        ON PR.UserId = U.Id AND PR.DeviceId = @stationId
    LEFT JOIN WeatherDevices WD
        ON WD.DeviceId = @stationId
WHERE U.Id = @userId;
";
        return await connection.QueryFirstOrDefaultAsync<StationUserPermissionDto>(
            sql,
            new
            {
                stationId,
                userId
            });
    }

    public async Task AddPermissionRequestAsync(int stationId, string userId, PermissionStatus status, DateTime createdAt)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @"
INSERT INTO [stationsAccess].[StationPermissionRequests]
    (UserId,
    DeviceId,
    Status,
    ChangeDate)
VALUES
    (@userId, @stationId, @status, @createdAt);
";
        await connection.ExecuteAsync(
            sql,
           new
           {
               userId,
               stationId,
               status,
               createdAt
           });
    }

    public async Task<(IEnumerable<UsersPermissionRequestDto> requests, int totalItems)> GetPermissionRequestsAsync(
        int pageNumber, int pageSize, string userId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT
    SP.Id AS {nameof(UsersPermissionRequestDto.Id)},
    SP.UserId AS {nameof(UsersPermissionRequestDto.UserId)},
    SP.DeviceId AS {nameof(UsersPermissionRequestDto.DeviceId)},
    M.Username AS {nameof(UsersPermissionRequestDto.DeviceName)},
    SP.Status AS {nameof(UsersPermissionRequestDto.PermissionStatus)},
    SP.ChangeDate AS {nameof(UsersPermissionRequestDto.ChangeDate)}
FROM [stationsAccess].[StationPermissionRequests] SP
    INNER JOIN [identity].[Devices] D
        ON D.Id = SP.DeviceId
    INNER JOIN [identity].[MqttClients] M
        ON M.Id = D.MqttClientId
WHERE
    SP.UserId = @userId
ORDER BY SP.ChangeDate
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY;

SELECT COUNT(*) 
FROM [stationsAccess].[StationPermissionRequests]
WHERE UserId = @userId;
";
        await using var multi = await connection.QueryMultipleAsync(
            sql,
            new
            {
                OffsetRows = pageSize * (pageNumber - 1),
                FetchRows = pageSize,
                userId
            });

        var requests = await multi.ReadAsync<UsersPermissionRequestDto>();
        var totalItems = await multi.ReadFirstAsync<int>();

        return (requests, totalItems);
    }

    public async Task<UsersPermissionDto?> GetUsersPermissionAsync(string userId, int deviceId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT
    Id AS {nameof(UsersPermissionDto.Id)},
    UserId AS {nameof(UsersPermissionDto.UserId)},
    DeviceId AS {nameof(UsersPermissionDto.DeviceId)}
FROM [stationsAccess].[StationsPermissions]
WHERE
    UserId = @userId
    AND DeviceId = @deviceId;
";
        return await connection.QueryFirstOrDefaultAsync(
            sql,
            new
            {
                userId,
                deviceId
            });
    }
}
