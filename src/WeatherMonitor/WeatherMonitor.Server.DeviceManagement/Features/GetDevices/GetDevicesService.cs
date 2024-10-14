using WeatherMonitor.Server.SharedKernel;

namespace WeatherMonitor.Server.DeviceManagement.Features.GetDevices;

internal interface IGetDevicesService
{
    Task<Result<PageResult<GetDeviceResponse>>> Handle();
}

internal class GetDevicesService : IGetDevicesService
{
    public async Task<Result<PageResult<GetDeviceResponse>>> Handle()
    {
        throw new NotImplementedException();

    }
}
