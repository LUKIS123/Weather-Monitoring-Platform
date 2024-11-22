using Dapper;
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

    public async Task<(IEnumerable<PendingPermissionDto> Stations, int totalItems)> GetPendingPermissionRequestsAsync(int pageNumber, int pageSize, PermissionStatus pending)
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
}
