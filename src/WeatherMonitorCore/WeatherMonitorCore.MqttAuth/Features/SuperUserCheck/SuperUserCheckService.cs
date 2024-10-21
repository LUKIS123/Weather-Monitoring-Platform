using WeatherMonitorCore.MqttAuth.Infrastructure;
using WeatherMonitorCore.MqttAuth.Infrastructure.Models;
using WeatherMonitorCore.SharedKernel;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.MqttAuth.Features.SuperUserCheck;

internal interface ISuperUserCheckService
{
    Task<Result> Handle(SuperUserCheckRequest superUserCheckRequest);
}

internal class SuperUserCheckService : ISuperUserCheckService
{
    private readonly IMqttClientAuthenticationRepository _mqttClientAuthenticationRepository;

    public SuperUserCheckService(IMqttClientAuthenticationRepository mqttClientAuthenticationRepository)
    {
        _mqttClientAuthenticationRepository = mqttClientAuthenticationRepository;
    }

    public async Task<Result> Handle(SuperUserCheckRequest superUserCheckRequest)
    {
        var mqttCredentials = await _mqttClientAuthenticationRepository.GetMqttSuperuserAsync(superUserCheckRequest.Username);

        if (mqttCredentials.Equals(default))
        {
            return Result.OnError(new ForbidException($"User:{superUserCheckRequest.Username} not found"));
        }

        if (!CheckSuperuser(mqttCredentials, superUserCheckRequest))
        {
            return Result.OnError(new ForbidException($"User:{superUserCheckRequest.Username} is not a superuser"));
        }

        return Result.OnSuccess();
    }

    private static bool CheckSuperuser(
        SuperUserCheckDto superUserCheckDto,
        SuperUserCheckRequest superUserCheckRequest) =>
        superUserCheckDto.IsSuperUser
        && string.Equals(superUserCheckDto.Username, superUserCheckRequest.Username);
}
