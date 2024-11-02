using WeatherMonitor.Server.DeviceManagement.Infrastructure.Models;

namespace WeatherMonitor.Server.DeviceManagement.Infrastructure;
public interface IDeviceManagementRepository
{
    public Task<(IEnumerable<GetDeviceInfoDto> DeviceInfos, int totalItems)> GetDevicesAsync(int pageSize, int pageNumber);
}
