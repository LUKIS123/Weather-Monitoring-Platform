using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.SharedKernel;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.DeviceManagement.Features.GetDeviceMqttCredentials;

internal interface IGetMqttCredentialsService
{
    Task<Result<DeviceMqttCredentialsResponse>> Handle(int deviceId);
}

internal class GetMqttCredentialsService : IGetMqttCredentialsService
{
    private readonly IDeviceManagementRepository _deviceManagementRepository;
    private readonly IAesEncryptionHelper _aesEncryptionHelper;

    public GetMqttCredentialsService(IDeviceManagementRepository deviceManagementRepository, IAesEncryptionHelper aesEncryptionHelper)
    {
        _deviceManagementRepository = deviceManagementRepository;
        _aesEncryptionHelper = aesEncryptionHelper;
    }

    public async Task<Result<DeviceMqttCredentialsResponse>> Handle(int deviceId)
    {
        var device = await _deviceManagementRepository.GetDeviceByIdAsync(deviceId);

        if (string.IsNullOrEmpty(device.ClientId)
            || string.IsNullOrEmpty(device.Username)
            || string.IsNullOrEmpty(device.Password)
            || string.IsNullOrEmpty(device.Topic))
        {
            return new NotFoundException($"Device with id:{deviceId} not found");
        }

        var plainTextPassword = _aesEncryptionHelper.Decrypt(device.Password);

        return new DeviceMqttCredentialsResponse(
            device.Id,
            device.Username,
            plainTextPassword,
            device.ClientId,
            device.Topic
        );
    }
}
