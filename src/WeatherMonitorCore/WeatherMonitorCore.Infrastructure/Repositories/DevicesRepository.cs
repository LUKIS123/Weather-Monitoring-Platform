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
}
