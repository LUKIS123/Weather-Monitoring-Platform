using Dapper;
using WeatherMonitor.Server.DeviceManagement.Infrastructure;
using WeatherMonitor.Server.DeviceManagement.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;

namespace WeatherMonitor.Server.Infrastructure.Repositories;
internal class DevicesRepository : IDeviceManagementRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DevicesRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<(IEnumerable<GetDeviceInfoDto> DevifeInfos, int totalItems)> GetDevicesAsync(int pageSize, int pageNumber)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT D.Id AS {nameof(GetDeviceInfoDto.Id)},
    D.GoogleMapsPlusCode AS {nameof(GetDeviceInfoDto.GoogleMapsPlusCode)},
    D.DeviceExtraInfo AS {nameof(GetDeviceInfoDto.DeviceExtraInfo)},
    D.IsActive AS {nameof(GetDeviceInfoDto.IsActive)},
    D.MqttClientId AS {nameof(GetDeviceInfoDto.MqttClientId)},
    C.Username AS {nameof(GetDeviceInfoDto.MqttUsername)},
    C.Password AS {nameof(GetDeviceInfoDto.Password)},
    C.ClientId AS {nameof(GetDeviceInfoDto.MqttBrokerClientId)},
    C.IsSuperUser AS {nameof(GetDeviceInfoDto.IsSuperUser)}
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
        var deviceInfos = await multi.ReadAsync<GetDeviceInfoDto>();
        var totalItems = await multi.ReadFirstAsync<int>();

        return (deviceInfos, totalItems);
    }
}
