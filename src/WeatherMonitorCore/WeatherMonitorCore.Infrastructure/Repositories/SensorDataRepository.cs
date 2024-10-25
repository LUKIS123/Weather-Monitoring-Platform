using Dapper;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Models;
using WeatherMonitorCore.MqttDataSubscriberService.Interfaces.Repositories;

namespace WeatherMonitorCore.Infrastructure.Repositories;
internal class SensorDataRepository : ISensorDataRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SensorDataRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task AddSensorDataAsync(SensorData sensorData, string topic)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        await connection.ExecuteAsync(@"
DECLARE @TargetDeviceId INT;
SET @TargetDeviceId = 
    (SELECT
        D.Id
    FROM [identity].[Devices] D
        INNER JOIN [identity].[MqttClients] MC ON D.MqttClientId = MC.Id
        INNER JOIN [identity].[MqttClientsAllowedTopics] MAT ON MC.Id = MAT.ClientId
        INNER JOIN [identity].[MqttTopics] T ON MAT.TopicId = T.Id
    WHERE T.Topic=@topic);

INSERT INTO [weatherData].[SensorsMeasurements]
    (
    MeasuredAt,
    Humidity,
    Temperature,
    AirPressure,
    PM1_0,
    PM2_5,
    PM10,
    DeviceId
    )
VALUES
    (
    @measuredAt,
    @humidity,
    @temperature,
    @airPressure,
    @pm1_0,
    @pm2_5,
    @pm10,
    @TargetDeviceId
);
", new
        {
            topic,
            measuredAt = sensorData.MeasuredAt,
            humidity = sensorData.Humidity,
            temperature = sensorData.Temperature,
            airPressure = sensorData.AirPressure,
            pm1_0 = sensorData.PM1_0,
            pm2_5 = sensorData.PM2_5,
            pm10 = sensorData.PM10
        });
    }
}
