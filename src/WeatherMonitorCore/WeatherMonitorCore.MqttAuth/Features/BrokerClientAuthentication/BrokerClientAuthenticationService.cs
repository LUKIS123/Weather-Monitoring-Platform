using WeatherMonitorCore.MqttAuth.Infrastructure;
using WeatherMonitorCore.MqttAuth.Infrastructure.Models;
using WeatherMonitorCore.SharedKernel;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.MqttAuth.Features.BrokerClientAuthentication;

internal interface IBrokerClientAuthenticationService
{
    Task<Result> Handle(AuthenticationRequest authenticationRequest);
}

internal class BrokerClientAuthenticationService : IBrokerClientAuthenticationService
{
    private readonly IMqttClientAuthenticationRepository _mqttClientAuthenticationRepository;

    public BrokerClientAuthenticationService(IMqttClientAuthenticationRepository mqttClientAuthenticationRepository)
    {
        _mqttClientAuthenticationRepository = mqttClientAuthenticationRepository;
    }

    public async Task<Result> Handle(AuthenticationRequest authenticationRequest)
    {
        var authenticationDto = await _mqttClientAuthenticationRepository.GetMqttCredentialsAsync(
            authenticationRequest.Username, authenticationRequest.ClientId);

        if (authenticationDto.Equals(default))
        {
            return Result.OnError(new ForbidException($"User:{authenticationRequest.Username} not found"));
        }

        if (!Authenticate(authenticationDto, authenticationRequest))
        {
            return Result.OnError(new ForbidException($"User:{authenticationRequest.Username} not authentication failed"));
        }

        return Result.OnSuccess();
    }

    private static bool Authenticate(
        BrokerClientAuthenticationDto authDto,
        AuthenticationRequest authenticationRequest) =>
        string.Equals(authDto.Username, authenticationRequest.Username)
        && string.Equals(authDto.ClientId, authenticationRequest.ClientId)
        && string.Equals(authDto.Password, authenticationRequest.Password);
}
