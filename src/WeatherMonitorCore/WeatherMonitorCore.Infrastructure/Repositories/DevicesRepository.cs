using Dapper;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
using WeatherMonitorCore.Interfaces;

namespace WeatherMonitorCore.Infrastructure.Repositories;
internal class DevicesRepository : IDeviceManagementRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DevicesRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<int> RegisterDeviceAsync(RegisterDeviceDto createDevice)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var createdId = await connection.ExecuteScalarAsync<int>(@"
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
    @mqttClientId,
    @username,
    @password,
    @clientId,
    @isSuperUser
);

INSERT INTO [identity].[MqttTopics]
    (
    Id,
    Topic
    )
VALUES
    (
    @mqttTopicId,
    @topic
);

INSERT INTO [identity].[MqttClientsAllowedTopics]
    (
    Id,
    ClientId,
    TopicId
    )
VALUES
    (
    @allowedTopicId,
    @mqttClientId,
    @mqttTopicId
);

INSERT INTO [identity].[MqttClientsTopicsPermissions]
    (
    AllowedTopicId,
    ActionType
    )
VALUES
    (
    @allowedTopicId,
    @actionType
);

INSERT INTO [identity].[Devices]
    (
    GoogleMapsPlusCode,
    DeviceExtraInfo,
    IsActive,
    MqttClientId
    )
OUTPUT
    INSERTED.Id
VALUES
    (
    @googleMapsPlusCode,
    @deviceExtraInfo,
    @isActivate,
    @mqttClientId
);
", new
        {
            mqttClientId = createDevice.MqttClient.Id,
            username = createDevice.MqttClient.Username,
            password = createDevice.MqttClient.Password,
            clientId = createDevice.MqttClient.ClientId,
            isSuperUser = createDevice.MqttClient.IsSuperUser,
            mqttTopicId = createDevice.MqttTopic.Id,
            topic = createDevice.MqttTopic.Topic,
            allowedTopicId = createDevice.MqttTopic.AllowedTopicId,
            actionType = createDevice.MqttTopic.Permission,
            googleMapsPlusCode = createDevice.CreateDevice.GoogleMapsPlusCode,
            deviceExtraInfo = createDevice.CreateDevice.DeviceExtraInfo,
            isActivate = createDevice.CreateDevice.IsActivate
        });

        return createdId;
    }

    public async Task<MqttCredentialsDto> GetDeviceByIdAsync(int deviceId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var credentials = await connection.QueryFirstOrDefaultAsync<MqttCredentialsDto>(@$"
SELECT TOP 1
    D.Id AS {nameof(MqttCredentialsDto.Id)},
    MC.Username AS {nameof(MqttCredentialsDto.Username)},
    MC.Password AS {nameof(MqttCredentialsDto.Password)},
    MC.ClientId AS {nameof(MqttCredentialsDto.ClientId)},
    T.Topic AS {nameof(MqttCredentialsDto.Topic)}
FROM [identity].[Devices] D
    INNER JOIN [identity].[MqttClients] MC ON D.MqttClientId = MC.Id
    INNER JOIN [identity].[MqttClientsAllowedTopics] MAT ON MC.Id = MAT.ClientId
    INNER JOIN [identity].[MqttTopics] T ON MAT.TopicId = T.Id
WHERE D.Id = @deviceId
", new { deviceId });

        return credentials;
    }
}



// DECLARE @TargetMqttId UNIQUEIDENTIFIER;
// SET @TargetMqttId = (SELECT MqttClientId
// FROM [WeatherMonitor].[identity].[Devices]
// WHERE Id=4);
// 
// DELETE
// FROM [WeatherMonitor].[identity].[Devices]
// WHERE Id=4;
// 
// DECLARE @TargetTopicId UNIQUEIDENTIFIER;
// SET @TargetTopicId = (SELECT TopicId
// FROM [WeatherMonitor].[identity].[MqttClientsAllowedTopics]
// WHERE ClientId='');
// 
// DELETE
// FROM [WeatherMonitor].[identity].[MqttClients]
// WHERE Id=@TargetMqttId;
// 
// DELETE
// FROM [WeatherMonitor].[identity].[MqttTopics]
// WHERE Id=@TargetTopicId;