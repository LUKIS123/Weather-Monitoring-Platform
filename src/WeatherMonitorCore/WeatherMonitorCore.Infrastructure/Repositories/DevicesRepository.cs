using Dapper;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.SharedKernel.Infrastructure.Models;

namespace WeatherMonitorCore.Infrastructure.Repositories;
internal class DevicesRepository : IDeviceManagementRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DevicesRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<CreatedDeviceDto> RegisterDeviceAsync(RegisterDeviceDto createDevice)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<BasicUserAuthorizationDto>(@$"
INSERT INTO [identity].[MqttClients]
(
    Id,
    Username,
    Password,
    ClientId,
    IsSuperUser
    )
VALUES
    (
    '',
    '',
    '',
    '',
    0
);

INSERT INTO [identity].[MqttTopics]
    (
    Id,
    Topic
    )
VALUES
    (
    '',
    ''
);

INSERT INTO [identity].[MqttClientsAllowedTopics]
    (
    Id,
    ClientId,
    TopicId
    )
VALUES
    (
    '',
    '',
    ''
);

INSERT INTO [identity].[MqttClientsTopicsPermissions]
    (
    AllowedTopicId,
    ActionType
    )
VALUES
    (
    '',
    2
);

INSERT INTO [identity].[Devices]
    (
    Id,
    GoogleMapsPlusCode,
    MqttClientId
    )
VALUES
    (
    '',
    '',
    ''
);
", new { userId });

        return null;
    }
}
