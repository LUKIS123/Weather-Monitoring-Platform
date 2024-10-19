using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
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

    public GetMqttCredentialsService(IDeviceManagementRepository deviceManagementRepository)
    {
        _deviceManagementRepository = deviceManagementRepository;
    }

    public async Task<Result<DeviceMqttCredentialsResponse>> Handle(int deviceId)
    {
        var device = await _deviceManagementRepository.GetDeviceByIdAsync(deviceId);

        if (string.IsNullOrEmpty(device.ClientId)
            || string.IsNullOrEmpty(device.Username)
            || string.IsNullOrEmpty(device.Password))
        {
            return new NotFoundException($"Device with id:{deviceId} not found");
        }

        return new DeviceMqttCredentialsResponse(
            device.Id,
            device.Username,
            device.Password,
            device.ClientId,
            device.Topic
        );
    }
}
