using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;
public interface IMqttClientsRepository
{
    Task CreateSuperUserAsync(CreateSuperUserDto superUser);
    Task RemoveSuperUserAsync(Guid id);
    Task<List<MqttSubscriptionsInfo>> GetDevicesTopicsAsync();
}
