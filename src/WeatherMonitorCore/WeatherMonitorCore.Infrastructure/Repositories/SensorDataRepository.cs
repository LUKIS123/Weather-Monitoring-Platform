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

INSERT INTO HourlyWeatherSummary
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

// public readonly record struct SensorData
// {
//     public long Id { get; init; }
//     public DateTime MeasuredAt { get; init; }
//     public float Humidity { get; init; }
//     public float Temperature { get; init; }
//     public float AirPressure { get; init; }
//     public float? PM1_0 { get; init; }
//     public float? PM2_5 { get; init; }
//     public float? PM10 { get; init; }
//     public int DeviceId { get; init; }
// }

// {
// "MeasuredAt": "2024-10-25T14:30:00Z",
// "Humidity": 55.3,
// "Temperature": 22.1,
// "AirPressure": 1013.25,
// "PM1_0": 12.4,
// "PM2_5": 18.9,
// "PM10": 25.7
// }
