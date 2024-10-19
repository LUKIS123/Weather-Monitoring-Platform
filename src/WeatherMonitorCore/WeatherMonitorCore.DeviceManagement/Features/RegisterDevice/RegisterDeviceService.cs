using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.Contract.Shared;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Utils;
using WeatherMonitorCore.SharedKernel;

namespace WeatherMonitorCore.DeviceManagement.Features.RegisterDevice;

internal interface IRegisterDeviceService
{
    Task<Result<CreateDeviceResponse>> Handle(RegisterDeviceRequest request);
}

internal class RegisterDeviceService : IRegisterDeviceService
{
    private readonly IDeviceManagementRepository _deviceManagementRepository;
    private readonly IPasswordGeneratorService _passwordGeneratorService;
    private readonly IDeviceCredentialsGenerator _credentialsGenerator;

    private const ActionType DefaultActionType = ActionType.Write;
    private const bool DefaultIsActivate = false;
    private const bool DefaultIsSuperUser = false;

    public RegisterDeviceService(
        IDeviceManagementRepository deviceManagementRepository,
        IPasswordGeneratorService passwordGeneratorService,
        IDeviceCredentialsGenerator credentialsGenerator)
    {
        _deviceManagementRepository = deviceManagementRepository;
        _passwordGeneratorService = passwordGeneratorService;
        _credentialsGenerator = credentialsGenerator;
    }

    public async Task<Result<CreateDeviceResponse>> Handle(RegisterDeviceRequest request)
    {
        var registerDeviceDto = MapRequestToDeviceDto(request);
        var deviceId = await _deviceManagementRepository.RegisterDeviceAsync(registerDeviceDto);

        return new CreateDeviceResponse(
            deviceId,
            registerDeviceDto.CreateDevice.GoogleMapsPlusCode,
            registerDeviceDto.CreateDevice.DeviceExtraInfo,
            registerDeviceDto.CreateDevice.IsActivate,
            registerDeviceDto.MqttClient.Username,
            registerDeviceDto.MqttClient.Password,
            registerDeviceDto.MqttClient.ClientId,
            registerDeviceDto.MqttTopic.Topic
        );
    }

    private RegisterDeviceDto MapRequestToDeviceDto(RegisterDeviceRequest request)
    {
        var mqttClientGuid = Guid.NewGuid();
        var mqttTopicGuid = Guid.NewGuid();
        var mqttAllowedTopicGuid = Guid.NewGuid();

        var createDevice = new CreateDeviceDto(
            request.GoogleMapsPlusCode,
            request.DeviceExtraInfo,
            DefaultIsActivate
        );

        var deviceMqttClientCredentials = _credentialsGenerator.GenerateDeviceCredentials(
            request.MqttUsername, mqttTopicGuid);

        var createMqttClient = new CreateMqttClientDto(
            mqttClientGuid,
            request.MqttUsername,
            _passwordGeneratorService.GeneratePassword(),
            deviceMqttClientCredentials.ClientId,
            DefaultIsSuperUser
        );

        var createMqttTopic = new CreateMqttTopicDto(
            mqttTopicGuid,
            deviceMqttClientCredentials.Topic,
            mqttAllowedTopicGuid,
            DefaultActionType
        );

        return new RegisterDeviceDto(
            createMqttClient,
            createMqttTopic,
            createDevice
        );
    }
}
