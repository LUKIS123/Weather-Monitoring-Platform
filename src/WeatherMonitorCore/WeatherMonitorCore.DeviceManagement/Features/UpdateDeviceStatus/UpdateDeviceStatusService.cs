using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.SharedKernel;

namespace WeatherMonitorCore.DeviceManagement.Features.UpdateDeviceStatus;

internal interface IUpdateDeviceStatusService
{
    Task<Result> Handle(UpdateDeviceStatusRequest requests);
}

internal class UpdateDeviceStatusService : IUpdateDeviceStatusService
{
    private readonly IDeviceManagementRepository _deviceManagementRepository;

    public UpdateDeviceStatusService(IDeviceManagementRepository deviceManagementRepository)
    {
        _deviceManagementRepository = deviceManagementRepository;
    }

    public async Task<Result> Handle(UpdateDeviceStatusRequest request)
    {
        if (!request.StationsUpdates.Any())
        {
            return Result.OnSuccess();
        }

        var tasks = request.StationsUpdates
            .GroupBy(r => r.SetActive)
            .Select(async group =>
            {
                var deviceIds = group.Select(r => r.DeviceId).ToArray();
                await _deviceManagementRepository.BulkUpdateDevicesStatusAsync(deviceIds, group.Key);
            });

        await Task.WhenAll(tasks);
        return Result.OnSuccess();
    }
}
