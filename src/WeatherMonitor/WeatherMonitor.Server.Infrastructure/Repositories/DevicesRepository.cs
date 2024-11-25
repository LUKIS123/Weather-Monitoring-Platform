using Dapper;
using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.DeviceManagement.Infrastructure;
using WeatherMonitor.Server.DeviceManagement.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;

namespace WeatherMonitor.Server.Infrastructure.Repositories;
internal class DevicesRepository : IDeviceManagementRepository, IWeatherStationsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DevicesRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<(IEnumerable<GetDeviceInfoDto> DeviceInfos, int totalItems)> GetDevicesAsync(int pageSize, int pageNumber)
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

    public async Task<(IEnumerable<GetStationResponse> Stations, int totalItems)> GetStationsAsync(int pageSize, int pageNumber)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT
    C.Username AS {nameof(GetStationResponse.Username)},
    D.Id AS {nameof(GetStationResponse.DeviceId)},
    D.GoogleMapsPlusCode AS {nameof(GetStationResponse.GoogleMapsPlusCode)},
    D.DeviceExtraInfo AS {nameof(GetStationResponse.DeviceExtraInfo)},
    D.IsActive AS {nameof(GetStationResponse.IsActive)},
    SM.Temperature AS {nameof(GetStationResponse.Temperature)},
    SM.Humidity AS {nameof(GetStationResponse.Humidity)},
    SM.AirPressure AS {nameof(GetStationResponse.AirPressure)},
    SM.Altitude AS {nameof(GetStationResponse.Altitude)},
    SM.PM10 AS {nameof(GetStationResponse.PM10)},
    SM.PM1_0 AS {nameof(GetStationResponse.PM1_0)},
    SM.PM2_5 AS {nameof(GetStationResponse.PM2_5)},
    SM.ReceivedAt AS {nameof(GetStationResponse.ReceivedAt)}
FROM [identity].[Devices] D
    LEFT JOIN [identity].[MqttClients] C ON D.MqttClientId = C.Id
    OUTER APPLY (
        SELECT TOP 1
        *
    FROM [weatherData].[SensorsMeasurements] SM
    WHERE SM.DeviceId = D.Id
    ORDER BY SM.ReceivedAt DESC
    ) SM
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
        var getStationResponses = await multi.ReadAsync<GetStationResponse>();
        var totalItems = await multi.ReadFirstAsync<int>();

        return (getStationResponses, totalItems);
    }

    public async Task<(IEnumerable<GetStationResponse> Stations, int totalItems)> GetStationsAsync(int pageSize, int pageNumber, string userId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT
    C.Username AS {nameof(GetStationResponse.Username)},
    D.Id AS {nameof(GetStationResponse.DeviceId)},
    D.GoogleMapsPlusCode AS {nameof(GetStationResponse.GoogleMapsPlusCode)},
    D.DeviceExtraInfo AS {nameof(GetStationResponse.DeviceExtraInfo)},
    D.IsActive AS {nameof(GetStationResponse.IsActive)},
    SM.Temperature AS {nameof(GetStationResponse.Temperature)},
    SM.Humidity AS {nameof(GetStationResponse.Humidity)},
    SM.AirPressure AS {nameof(GetStationResponse.AirPressure)},
    SM.Altitude AS {nameof(GetStationResponse.Altitude)},
    SM.PM10 AS {nameof(GetStationResponse.PM10)},
    SM.PM1_0 AS {nameof(GetStationResponse.PM1_0)},
    SM.PM2_5 AS {nameof(GetStationResponse.PM2_5)},
    SM.ReceivedAt AS {nameof(GetStationResponse.ReceivedAt)}
FROM [identity].[Devices] D
    LEFT JOIN [stationsAccess].[StationsPermissions] SP ON D.Id = SP.DeviceId
    LEFT JOIN [identity].[MqttClients] C ON D.MqttClientId = C.Id
    OUTER APPLY (
        SELECT TOP 1
        *
    FROM [weatherData].[SensorsMeasurements] SM
    WHERE SM.DeviceId = D.Id
    ORDER BY SM.ReceivedAt DESC
    ) SM
WHERE SP.UserId = @UserId
ORDER BY D.Id
OFFSET @OffsetRows ROWS
FETCH NEXT @FetchRows ROWS ONLY;

SELECT COUNT(*) 
FROM [identity].[Devices] D
    LEFT JOIN [stationsAccess].[StationsPermissions] SP ON D.Id = SP.DeviceId
WHERE SP.UserId = @UserId;
";
        await using var multi = await connection.QueryMultipleAsync(
            sql,
            new
            {
                OffsetRows = pageSize * (pageNumber - 1),
                FetchRows = pageSize,
                UserId = userId
            });
        var getStationResponses = await multi.ReadAsync<GetStationResponse>();
        var totalItems = await multi.ReadFirstAsync<int>();

        return (getStationResponses, totalItems);
    }
}
