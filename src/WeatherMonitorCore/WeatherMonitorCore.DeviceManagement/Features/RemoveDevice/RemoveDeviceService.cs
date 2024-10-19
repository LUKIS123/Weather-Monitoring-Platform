using WeatherMonitorCore.DeviceManagement.Infrastructure;
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

    public RemoveDeviceService(IDeviceManagementRepository deviceManagementRepository)
    {
        _deviceManagementRepository = deviceManagementRepository;
    }

    public async Task<Result> Handle(int deviceId)
    {
        var device = await _deviceManagementRepository.GetDeviceByIdAsync(deviceId);
        if (device.Equals(default))
        {
            return Result.OnError(new NotFoundException($"Device with id:{deviceId} not found"));
        }

        await _deviceManagementRepository.RemoveDeviceAsync(deviceId);

        return Result.OnSuccess();
    }
}
