using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;

namespace WeatherMonitorCore.DeviceManagement.Infrastructure;
public interface IDeviceManagementRepository
{
    public Task<RegisteredDeviceDto> RegisterDeviceAsync(CreateDeviceDto createDevice);
}
