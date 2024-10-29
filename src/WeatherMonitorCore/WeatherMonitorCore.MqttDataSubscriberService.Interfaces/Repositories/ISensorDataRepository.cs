using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;

public interface ISensorDataRepository
{
    Task AddSensorDataAsync(MeasurementsLog sensorData, string topic);
}
