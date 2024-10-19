using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;

namespace WeatherMonitorCore.DeviceManagement.Infrastructure;
public interface IDeviceManagementRepository
{
    Task<int> RegisterDeviceAsync(RegisterDeviceDto createDevice);
    Task<MqttCredentialsDto> GetDeviceByIdAsync(int deviceId);
    Task RemoveDeviceAsync(int deviceId);
}
