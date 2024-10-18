using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
public readonly record struct RegisterDeviceDto(
    CreateMqttClientDto MqttClient,
    CreateMqttTopicDto MqttTopic,
    CreateDeviceDto CreateDevice
    );

public readonly record struct CreateMqttClientDto(
    Guid Id,
    string Username,
    string Password,
    string ClientId,
    bool IsSuperUser
    );

public readonly record struct CreateMqttTopicDto(
    Guid Id,
    string Topic,
    Guid AllowedTopicId,
    ActionType Permission
    );

public readonly record struct CreateDeviceDto(
    string GoogleMapsPlusCode,
    string? DeviceExtraInfo,
    bool IsActivate
    );
