using WeatherMonitorCore.MqttDataSubscriberService.Infrastructure.Models;

namespace WeatherMonitorCore.MqttDataSubscriberService.Infrastructure;

internal interface ISensorDataService
{
    public Task AddWeatherDataAsync(SensorData weatherData);
}
