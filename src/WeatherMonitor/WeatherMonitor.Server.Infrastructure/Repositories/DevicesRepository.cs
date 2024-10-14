using WeatherMonitor.Server.DeviceManagement.Infrastructure;
using WeatherMonitor.Server.DeviceManagement.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;

namespace WeatherMonitor.Server.Infrastructure.Repositories;
internal class DevicesRepository : IDeviceManagementRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DevicesRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public Task<(IEnumerable<GetDeviceInfoDto> DevifeInfos, int totalItems)> GetDevicesAsync(int pageSize, int pageNumber)
    {
        throw new NotImplementedException();
    }
}
