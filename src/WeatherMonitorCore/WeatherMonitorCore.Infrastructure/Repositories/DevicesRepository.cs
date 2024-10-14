using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;

namespace WeatherMonitorCore.Infrastructure.Repositories;
internal class DevicesRepository : IDeviceManagementRepository
{
    public Task<RegisteredDeviceDto> RegisterDeviceAsync(CreateDeviceDto createDevice)
    {
        throw new NotImplementedException();
    }
}
