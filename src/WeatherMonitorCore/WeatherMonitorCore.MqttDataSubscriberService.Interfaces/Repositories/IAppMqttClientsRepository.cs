using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;
public interface IAppMqttClientsRepository
{
    Task CreateSuperUserAsync(CreateWorkerUserDto superUser);
    Task RemoveWorkerUserAsync(Guid id);
    Task<List<MqttSubscriptionsInfo>> GetDevicesTopicsAsync();
}
