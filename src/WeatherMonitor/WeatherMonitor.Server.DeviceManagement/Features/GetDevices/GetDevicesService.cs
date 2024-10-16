using WeatherMonitor.Server.DeviceManagement.Infrastructure;
using WeatherMonitor.Server.SharedKernel;

namespace WeatherMonitor.Server.DeviceManagement.Features.GetDevices;

internal interface IGetDevicesService
{
    Task<Result<PageResult<GetDeviceResponse>>> Handle(int pageNumber);
}

internal class GetDevicesService : IGetDevicesService
{
    private readonly IDeviceManagementRepository _repository;
    private const int PageSize = 30;

    public GetDevicesService(IDeviceManagementRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PageResult<GetDeviceResponse>>> Handle(int pageNumber)
    {
        var (deviceInfos, itemCount) = await _repository.GetDevicesAsync(PageSize, pageNumber);
        var devices = deviceInfos.Select(d => new GetDeviceResponse(
            d.GoogleMapsPlusCode,
            d.IsActive,
            d.DeviceExtraInfo,
            d.MqttUsername,
            d.MqttBrokerClientId));

        return new PageResult<GetDeviceResponse>(devices, itemCount, PageSize, pageNumber);
    }
}
