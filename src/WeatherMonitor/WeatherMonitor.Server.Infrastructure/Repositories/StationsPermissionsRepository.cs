using Dapper;
using WeatherMonitor.Server.Interfaces;
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
ORDER BY D.Id
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY;

SELECT COUNT(*) 
FROM [identity].[Devices];
";
        await using var multi = await connection.QueryMultipleAsync(
            sql,
            new
            {
                OffsetRows = pageSize * (pageNumber - 1),
                FetchRows = pageSize
            });
        var deviceInfos = await multi.ReadAsync<AvailableStation>();
        var totalItems = await multi.ReadFirstAsync<int>();

        return (deviceInfos, totalItems);
    }
}
