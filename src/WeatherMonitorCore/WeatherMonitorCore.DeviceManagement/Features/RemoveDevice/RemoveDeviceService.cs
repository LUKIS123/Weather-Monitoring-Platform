using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.Shared.MqttClient.Interfaces;
using WeatherMonitorCore.SharedKernel;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.DeviceManagement.Features.RemoveDevice;

internal interface IRemoveDeviceService
{
    Task<Result> Handle(int deviceId);
}

internal class RemoveDeviceService : IRemoveDeviceService
{
    private readonly IDeviceManagementRepository _deviceManagementRepository;
    private readonly ISubscriptionsManagingService _subscriptionsManagingService;

    public RemoveDeviceService(
        IDeviceManagementRepository deviceManagementRepository,
        ISubscriptionsManagingService subscriptionsManagingService)
    {
        _deviceManagementRepository = deviceManagementRepository;
        _subscriptionsManagingService = subscriptionsManagingService;
    }

    public async Task<Result> Handle(int deviceId)
    {
        var device = await _deviceManagementRepository.GetDeviceByIdAsync(deviceId);
        if (device.Equals(default))
        {
            return Result.OnError(new NotFoundException($"Device with id:{deviceId} not found"));
        }

        await _subscriptionsManagingService.RemoveTopicAsync(device.Topic, CancellationToken.None);
        await _deviceManagementRepository.RemoveDeviceAsync(deviceId);

        return Result.OnSuccess();
    }
}
