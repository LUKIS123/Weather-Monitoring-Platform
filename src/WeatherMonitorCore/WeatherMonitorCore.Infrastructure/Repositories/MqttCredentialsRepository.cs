using Dapper;
using WeatherMonitorCore.Contract.Shared;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.MqttAuth.Infrastructure;
using WeatherMonitorCore.MqttAuth.Infrastructure.Models;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;

namespace WeatherMonitorCore.Infrastructure.Repositories;
internal class MqttCredentialsRepository : IMqttClientAuthenticationRepository, IAppMqttClientsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MqttCredentialsRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<AclCheckDto> GetMqttAclAsync(string username, string clientId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        const string sql = @$"
SELECT
    C.Username AS {nameof(AclCheckDto.Username)},
    C.ClientId AS {nameof(AclCheckDto.ClientId)},
    T.Topic AS {nameof(AclCheckDto.Topic)},
    P.ActionType
FROM [identity].[MqttClients] C
    LEFT JOIN [identity].[MqttClientsAllowedTopics] MAT ON C.Id = MAT.ClientId
    LEFT JOIN [identity].[MqttTopics] T ON MAT.TopicId = T.Id
    LEFT JOIN [identity].[MqttClientsTopicsPermissions] P ON MAT.Id = P.AllowedTopicId
WHERE C.Username=@username AND C.ClientId=@clientId
";
        var actionTypes = new List<ActionType>();
        var result = await connection.QueryAsync<AclCheckDto, ActionType, AclCheckDto>(
            sql: sql,
            map: (acl, actionType) =>
            {
                actionTypes.Add(actionType);
                acl = acl with { AllowedActions = actionTypes };

                return acl;
            },
            param: new { username, clientId },
            splitOn: "ActionType");

        return result.FirstOrDefault();
    }

    public async Task<BrokerClientAuthenticationDto> GetMqttCredentialsAsync(string username, string clientId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<BrokerClientAuthenticationDto>(@$"
SELECT TOP 1
    Username AS {nameof(BrokerClientAuthenticationDto.Username)},
    Password AS {nameof(BrokerClientAuthenticationDto.Password)},
    ClientId AS {nameof(BrokerClientAuthenticationDto.ClientId)}
FROM [identity].[MqttClients]
WHERE Username=@username AND ClientId=@clientId
", new { username, clientId });

        return result;
    }

    public async Task<SuperUserCheckDto> GetMqttSuperuserAsync(string username)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<SuperUserCheckDto>(@$"
SELECT TOP 1
    Username AS {nameof(SuperUserCheckDto.Username)},
    IsSuperUser AS {nameof(SuperUserCheckDto.IsSuperUser)}
FROM [identity].[MqttClients]
WHERE Username=@username
", new { username });

        return result;
    }

    public async Task CreateSuperUserAsync(CreateWorkerUserDto superUser)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        await connection.ExecuteAsync(@"
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
)
", new
        {
            mqttClientId = superUser.Id,
            username = superUser.Username,
            password = superUser.Password,
            clientId = superUser.ClientId,
            isSuperUser = superUser.IsSuperUser,
        });
    }

    public async Task RemoveWorkerUserAsync(Guid id)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        await connection.ExecuteAsync(@"
DELETE FROM [identity].[MqttClients]
WHERE Id=@id
", new { id });
    }

    public async Task<List<MqttSubscriptionsInfo>> GetDevicesTopicsAsync()
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var subscriptionsInfos = await connection.QueryAsync<MqttSubscriptionsInfo>($@"
SELECT
    T.Topic AS {nameof(MqttSubscriptionsInfo.Topic)}
FROM [identity].[Devices] D
    INNER JOIN [identity].[MqttClients] MC ON D.MqttClientId = MC.Id
    INNER JOIN [identity].[MqttClientsAllowedTopics] MAT ON MC.Id = MAT.ClientId
    INNER JOIN [identity].[MqttTopics] T ON MAT.TopicId = T.Id
");
        return subscriptionsInfos.ToList();
    }
}
