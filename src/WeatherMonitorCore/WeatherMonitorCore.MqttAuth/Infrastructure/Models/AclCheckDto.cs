using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitorCore.MqttAuth.Infrastructure.Models;

public readonly record struct AclCheckDto(
    string Username,
    string ClientId,
    string Topic,
    List<ActionType> AllowedActions);