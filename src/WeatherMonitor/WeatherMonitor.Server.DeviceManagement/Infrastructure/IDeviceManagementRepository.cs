using WeatherMonitor.Server.DeviceManagement.Infrastructure.Models;

namespace WeatherMonitor.Server.DeviceManagement.Infrastructure;
public interface IDeviceManagementRepository
{
    public Task<(IEnumerable<GetDeviceInfoDto> DevifeInfos, int totalItems)> GetDevicesAsync(int pageSize, int pageNumber);
}
