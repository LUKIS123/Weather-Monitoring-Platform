using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;

public interface ISensorDataRepository
{
    Task AddSensorDataAsync(SensorData sensorData, string topic);
}
