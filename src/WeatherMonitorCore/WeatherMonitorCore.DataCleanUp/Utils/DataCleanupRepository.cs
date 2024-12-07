using Dapper;
using Microsoft.Data.SqlClient;
using WeatherMonitorCore.DataCleanUp.Settings;

namespace WeatherMonitorCore.DataCleanUp.Utils;

public interface IDataCleanupRepository
{
    Task<int> RemoveOldDataAsync(DateTime olderThan);
}

public class DataCleanupRepository : IDataCleanupRepository
{
    private readonly ConnectionSettings _connectionSettings;

    public DataCleanupRepository(ConnectionSettings settings)
    {
        _connectionSettings = settings;
    }

    public async Task<int> RemoveOldDataAsync(DateTime olderThan)
    {
        await using var connection = new SqlConnection(_connectionSettings.ConnectionString);
        await connection.OpenAsync();

        return await connection.ExecuteAsync(@"
DELETE FROM [weatherData].[SensorsMeasurements]
WHERE ReceivedAt < DATEADD(DAY, -31, @olderThan)
", new { olderThan });
    }
}
