using WeatherMonitorCore.MqttAuth.Infrastructure;
using WeatherMonitorCore.MqttAuth.Infrastructure.Models;
using WeatherMonitorCore.SharedKernel;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.MqttAuth.Features.AclCheck;

internal interface IAclCheckService
{
    Task<Result> Handle(AclCheckRequest aclCheckRequest);
}

internal class AclCheckService : IAclCheckService
{
    private readonly IMqttClientAuthenticationRepository _mqttClientAuthenticationRepository;

    public AclCheckService(IMqttClientAuthenticationRepository mqttClientAuthenticationRepository)
    {
        _mqttClientAuthenticationRepository = mqttClientAuthenticationRepository;
    }

    public async Task<Result> Handle(AclCheckRequest aclCheckRequest)
    {
        var mqttCredentials = await _mqttClientAuthenticationRepository.GetMqttAclAsync(aclCheckRequest.Username, aclCheckRequest.ClientId);

        if (mqttCredentials.Equals(default))
        {
            return Result.OnError(new ForbidException($"User:{aclCheckRequest.Username} not found"));
        }

        if (!CheckAcl(mqttCredentials, aclCheckRequest))
        {
            return Result.OnError(new ForbidException($"User:{aclCheckRequest.Username} not allowed to {aclCheckRequest.Action} on {aclCheckRequest.Topic}"));
        }

        return Result.OnSuccess();
    }

    private static bool CheckAcl(AclCheckDto aclCheckDto, AclCheckRequest aclCheckRequest) =>
        string.Equals(aclCheckDto.Username, aclCheckRequest.Username)
        && string.Equals(aclCheckDto.ClientId, aclCheckRequest.ClientId)
        && string.Equals(aclCheckDto.Topic, aclCheckRequest.Topic)
        && aclCheckDto.AllowedActions.Any(a => a == aclCheckRequest.Action);
}
