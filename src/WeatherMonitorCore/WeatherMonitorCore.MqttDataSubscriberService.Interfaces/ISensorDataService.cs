using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;

namespace WeatherMonitorCore.MqttDataSubscriberService.Interfaces;

internal interface ISensorDataService
{
    public Task AddWeatherDataAsync(SensorData weatherData);
}
